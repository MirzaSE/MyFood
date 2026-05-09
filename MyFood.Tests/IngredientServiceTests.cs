using AutoMapper;
using Moq;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Tests
{
    public class IngredientServiceTests
    {
        private readonly Mock<IIngredientRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly IngredientService _service;

        public IngredientServiceTests()
        {
            _service = new IngredientService(_repoMock.Object, _mapperMock.Object);
        }

        // GetAllAsync ---------------------------------------------------------

        [Fact]
        public async Task GetAllAsync_ReturnsMappedDtos()
        {
            var entities = new List<IngredientEntity>
            {
                new() { Id = 1, Name = "Apple", Unit = "g", CaloriesPerUnit = 0.52 }
            };
            var dtos = new List<IngredientDto>
            {
                new() { Id = 1, Name = "Apple", Unit = "g", CaloriesPerUnit = 0.52 }
            };
            _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
            _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

            var result = await _service.GetAllAsync(new QueryParameters());

            Assert.Single(result);
            Assert.Equal("Apple", result.First().Name);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmpty_WhenNoIngredients()
        {
            _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(new List<IngredientEntity>().AsQueryable());
            _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto>());

            var result = await _service.GetAllAsync(new QueryParameters());

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_PassesPaginationParametersToRepository()
        {
            var qp = new QueryParameters { Page = 3, PageCount = 7 };
            QueryParameters? captured = null;
            _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>()))
                .Callback<QueryParameters>(p => captured = p)
                .Returns(new List<IngredientEntity>().AsQueryable());
            _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto>());

            await _service.GetAllAsync(qp);

            Assert.NotNull(captured);
            Assert.Equal(3, captured!.Page);
            Assert.Equal(7, captured.PageCount);
        }

        // GetByIdAsync --------------------------------------------------------

        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenFound()
        {
            var entity = new IngredientEntity { Id = 2, Name = "Banana", Unit = "piece" };
            var dto = new IngredientDto { Id = 2, Name = "Banana", Unit = "piece" };
            _repoMock.Setup(r => r.GetSingle(2)).Returns(entity);
            _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

            var result = await _service.GetByIdAsync(2);

            Assert.NotNull(result);
            Assert.Equal("Banana", result!.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

            var result = await _service.GetByIdAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsArgumentOutOfRange_OnNegativeId()
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.GetByIdAsync(-1));
        }

        // CreateAsync ---------------------------------------------------------

        [Fact]
        public async Task CreateAsync_AddsAndReturnsDto()
        {
            var createDto = new IngredientCreateDto { Name = "Pear", Unit = "g", CaloriesPerUnit = 0.57 };
            var entity = new IngredientEntity { Id = 3, Name = "Pear", Unit = "g", CaloriesPerUnit = 0.57 };
            var dto = new IngredientDto { Id = 3, Name = "Pear", Unit = "g", CaloriesPerUnit = 0.57 };
            _repoMock.Setup(r => r.ExistsByName("Pear")).Returns(false);
            _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
            _repoMock.Setup(r => r.Save()).Returns(true);
            _repoMock.Setup(r => r.GetSingle(entity.Id)).Returns(entity);
            _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

            var result = await _service.CreateAsync(createDto);

            Assert.Equal("Pear", result.Name);
            _repoMock.Verify(r => r.Add(entity), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ThrowsArgumentNull_WhenNameNull()
        {
            var createDto = new IngredientCreateDto { Name = null, Unit = "g" };

            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_ThrowsInvalidOperation_OnDuplicateName()
        {
            var createDto = new IngredientCreateDto { Name = "Apple", Unit = "g" };
            _repoMock.Setup(r => r.ExistsByName("Apple")).Returns(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_MapsCreateDtoToEntityCorrectly()
        {
            var createDto = new IngredientCreateDto
            {
                Name = "Tomato",
                Unit = "g",
                CaloriesPerUnit = 0.18,
                Protein = 0.9,
                Carbs = 3.9,
                Fat = 0.2
            };
            var entity = new IngredientEntity { Id = 7, Name = "Tomato", Unit = "g" };
            _repoMock.Setup(r => r.ExistsByName("Tomato")).Returns(false);
            _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity).Verifiable();
            _repoMock.Setup(r => r.Save()).Returns(true);
            _repoMock.Setup(r => r.GetSingle(entity.Id)).Returns(entity);
            _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(new IngredientDto { Id = 7, Name = "Tomato", Unit = "g" });

            await _service.CreateAsync(createDto);

            _mapperMock.Verify(m => m.Map<IngredientEntity>(createDto), Times.Once);
        }

        // UpdateAsync ---------------------------------------------------------

        [Fact]
        public async Task UpdateAsync_UpdatesAndReturnsDto()
        {
            var updateDto = new IngredientUpdateDto { Name = "Updated", Unit = "g" };
            var entity = new IngredientEntity { Id = 4, Name = "Old", Unit = "g" };
            var updatedEntity = new IngredientEntity { Id = 4, Name = "Updated", Unit = "g" };
            var dto = new IngredientDto { Id = 4, Name = "Updated", Unit = "g" };

            _repoMock.Setup(r => r.GetSingle(4)).Returns(entity);
            _mapperMock.Setup(m => m.Map(updateDto, entity));
            _repoMock.Setup(r => r.Update(4, entity)).Returns(updatedEntity);
            _repoMock.Setup(r => r.Save()).Returns(true);
            _mapperMock.Setup(m => m.Map<IngredientDto>(updatedEntity)).Returns(dto);

            var result = await _service.UpdateAsync(4, updateDto);

            Assert.NotNull(result);
            Assert.Equal("Updated", result!.Name);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

            var result = await _service.UpdateAsync(99, new IngredientUpdateDto { Name = "X", Unit = "g" });

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_PartialUpdate_AppliesMapperToExistingEntity()
        {
            var updateDto = new IngredientUpdateDto { Name = "NewName", Unit = "g", CaloriesPerUnit = 1.5 };
            var existing = new IngredientEntity { Id = 5, Name = "OldName", Unit = "g", CaloriesPerUnit = 0.5 };

            _repoMock.Setup(r => r.GetSingle(5)).Returns(existing);
            _mapperMock.Setup(m => m.Map(updateDto, existing)).Verifiable();
            _repoMock.Setup(r => r.Update(5, existing)).Returns(existing);
            _repoMock.Setup(r => r.Save()).Returns(true);
            _mapperMock.Setup(m => m.Map<IngredientDto>(existing)).Returns(new IngredientDto { Id = 5, Name = "NewName", Unit = "g" });

            await _service.UpdateAsync(5, updateDto);

            _mapperMock.Verify(m => m.Map(updateDto, existing), Times.Once);
        }

        // DeleteAsync ---------------------------------------------------------

        [Fact]
        public async Task DeleteAsync_DeletesAndReturnsTrue()
        {
            var entity = new IngredientEntity { Id = 5, Name = "X", Unit = "g" };
            _repoMock.Setup(r => r.GetSingle(5)).Returns(entity);
            _repoMock.Setup(r => r.Save()).Returns(true);

            var result = await _service.DeleteAsync(5);

            Assert.True(result);
            _repoMock.Verify(r => r.Delete(5), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

            var result = await _service.DeleteAsync(99);

            Assert.False(result);
        }

        // SearchAsync ---------------------------------------------------------

        [Fact]
        public async Task SearchAsync_FindsMatches()
        {
            var matches = new List<IngredientEntity>
            {
                new() { Id = 1, Name = "Apple", Unit = "g" }
            };
            _repoMock.Setup(r => r.SearchByName("app")).Returns(matches);
            _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(matches))
                .Returns(new List<IngredientDto> { new() { Id = 1, Name = "Apple", Unit = "g" } });

            var result = await _service.SearchAsync("app");

            Assert.Single(result);
            Assert.Equal("Apple", result.First().Name);
        }

        [Fact]
        public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
        {
            _repoMock.Setup(r => r.SearchByName(It.IsAny<string>())).Returns(new List<IngredientEntity>());
            _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto>());

            var result = await _service.SearchAsync("zzz");

            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchAsync_IsCaseInsensitive()
        {
            var entities = new List<IngredientEntity>
            {
                new() { Id = 1, Name = "Apple", Unit = "g" }
            };
            // Repo SearchByName uses EF.Functions.Like which is case-insensitive in SQL Server collation;
            // the service forwards the term untouched.
            _repoMock.Setup(r => r.SearchByName("APPLE")).Returns(entities);
            _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities))
                .Returns(new List<IngredientDto> { new() { Id = 1, Name = "Apple", Unit = "g" } });

            var result = await _service.SearchAsync("APPLE");

            Assert.Single(result);
            Assert.Equal("Apple", result.First().Name);
            _repoMock.Verify(r => r.SearchByName("APPLE"), Times.Once);
        }
    }
}
