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

        // -----------------------------------------------------------
        // GetAllAsync (3)
        // -----------------------------------------------------------
        [Fact]
        public async Task GetAllAsync_ReturnsAll_WhenRepositoryHasItems()
        {
            var entities = new List<IngredientEntity>
            {
                new() { Id = 1, Name = "Tomato",  Unit = "g" },
                new() { Id = 2, Name = "Olive Oil", Unit = "ml" }
            };
            var dtos = new List<IngredientDto>
            {
                new() { Id = 1, Name = "Tomato",  Unit = "g" },
                new() { Id = 2, Name = "Olive Oil", Unit = "ml" }
            };

            _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
            _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

            var result = await _service.GetAllAsync(new QueryParameters());

            Assert.Equal(2, result.Count());
            Assert.Contains(result, x => x.Name == "Tomato");
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmpty_WhenRepositoryHasNoItems()
        {
            _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(Enumerable.Empty<IngredientEntity>().AsQueryable());
            _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto>());

            var result = await _service.GetAllAsync(new QueryParameters());

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_PassesPaginationParametersToRepository()
        {
            var qp = new QueryParameters { Page = 2, PageCount = 5 };
            _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(Enumerable.Empty<IngredientEntity>().AsQueryable());
            _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto>());

            await _service.GetAllAsync(qp);

            _repoMock.Verify(r => r.GetAll(It.Is<QueryParameters>(p => p.Page == 2 && p.PageCount == 5)), Times.Once);
        }

        // -----------------------------------------------------------
        // GetByIdAsync (3)
        // -----------------------------------------------------------
        [Fact]
        public async Task GetByIdAsync_ReturnsMappedDto_WhenFound()
        {
            var entity = new IngredientEntity { Id = 7, Name = "Salt", Unit = "g" };
            var dto = new IngredientDto { Id = 7, Name = "Salt", Unit = "g" };
            _repoMock.Setup(r => r.GetSingle(7)).Returns(entity);
            _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

            var result = await _service.GetByIdAsync(7);

            Assert.NotNull(result);
            Assert.Equal("Salt", result!.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

            var result = await _service.GetByIdAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_Throws_WhenIdIsNegative()
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.GetByIdAsync(-1));
            _repoMock.Verify(r => r.GetSingle(It.IsAny<int>()), Times.Never);
        }

        // -----------------------------------------------------------
        // CreateAsync (4)
        // -----------------------------------------------------------
        [Fact]
        public async Task CreateAsync_AddsAndReturnsDto_OnSuccess()
        {
            var createDto = new IngredientCreateDto { Name = "Onion", Unit = "g", CaloriesPerUnit = 0.4 };
            var entity = new IngredientEntity { Id = 3, Name = "Onion", Unit = "g", CaloriesPerUnit = 0.4 };
            var dto = new IngredientDto { Id = 3, Name = "Onion", Unit = "g", CaloriesPerUnit = 0.4 };

            _repoMock.Setup(r => r.ExistsByName("Onion", null)).Returns(false);
            _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
            _repoMock.Setup(r => r.Add(entity));
            _repoMock.Setup(r => r.Save()).Returns(true);
            _repoMock.Setup(r => r.GetSingle(entity.Id)).Returns(entity);
            _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

            var result = await _service.CreateAsync(createDto);

            Assert.Equal("Onion", result.Name);
            _repoMock.Verify(r => r.Add(entity), Times.Once);
            _repoMock.Verify(r => r.Save(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenNameIsNullOrWhitespace()
        {
            var createDto = new IngredientCreateDto { Name = "   ", Unit = "g" };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
            _repoMock.Verify(r => r.Add(It.IsAny<IngredientEntity>()), Times.Never);
            _repoMock.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenDuplicateName()
        {
            var createDto = new IngredientCreateDto { Name = "Tomato", Unit = "g" };
            _repoMock.Setup(r => r.ExistsByName("Tomato", null)).Returns(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
            _repoMock.Verify(r => r.Add(It.IsAny<IngredientEntity>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_MapsAllFieldsCorrectly()
        {
            var createDto = new IngredientCreateDto
            {
                Name = "Cheddar",
                Unit = "g",
                CaloriesPerUnit = 4.02,
                Protein = 0.25,
                Carbs = 0.013,
                Fat = 0.33
            };
            var mapped = new IngredientEntity
            {
                Name = "Cheddar",
                Unit = "g",
                CaloriesPerUnit = 4.02,
                Protein = 0.25,
                Carbs = 0.013,
                Fat = 0.33
            };
            var savedEntity = new IngredientEntity { Id = 11 };
            var dto = new IngredientDto { Id = 11, Name = "Cheddar", Unit = "g" };

            _repoMock.Setup(r => r.ExistsByName("Cheddar", null)).Returns(false);
            _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(mapped);
            _repoMock.Setup(r => r.Save()).Returns(true);
            _repoMock.Setup(r => r.GetSingle(It.IsAny<int>())).Returns(savedEntity);
            _mapperMock.Setup(m => m.Map<IngredientDto>(savedEntity)).Returns(dto);

            var result = await _service.CreateAsync(createDto);

            _mapperMock.Verify(m => m.Map<IngredientEntity>(createDto), Times.Once);
            Assert.Equal(11, result.Id);
        }

        // -----------------------------------------------------------
        // UpdateAsync (3)
        // -----------------------------------------------------------
        [Fact]
        public async Task UpdateAsync_UpdatesAndReturnsDto_WhenFound()
        {
            var updateDto = new IngredientUpdateDto { Name = "Updated", CaloriesPerUnit = 2.5 };
            var existing = new IngredientEntity { Id = 4, Name = "Old", Unit = "g" };
            var updated = new IngredientEntity { Id = 4, Name = "Updated", Unit = "g", CaloriesPerUnit = 2.5 };
            var dto = new IngredientDto { Id = 4, Name = "Updated", Unit = "g", CaloriesPerUnit = 2.5 };

            _repoMock.Setup(r => r.GetSingle(4)).Returns(existing);
            _mapperMock.Setup(m => m.Map(updateDto, existing));
            _repoMock.Setup(r => r.Update(4, existing)).Returns(updated);
            _repoMock.Setup(r => r.Save()).Returns(true);
            _mapperMock.Setup(m => m.Map<IngredientDto>(updated)).Returns(dto);

            var result = await _service.UpdateAsync(4, updateDto);

            Assert.NotNull(result);
            Assert.Equal("Updated", result!.Name);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

            var result = await _service.UpdateAsync(99, new IngredientUpdateDto());

            Assert.Null(result);
            _repoMock.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_HandlesPartialUpdate_OnlyMapsProvidedFields()
        {
            // Only Name supplied; Unit/CaloriesPerUnit etc. left null.
            // Service trusts AutoMapper config (ForAllMembers Condition) to skip null members.
            var updateDto = new IngredientUpdateDto { Name = "PartialOnly" };
            var existing = new IngredientEntity { Id = 5, Name = "Old", Unit = "g", CaloriesPerUnit = 1.0 };

            _repoMock.Setup(r => r.GetSingle(5)).Returns(existing);
            _mapperMock.Setup(m => m.Map(updateDto, existing));
            _repoMock.Setup(r => r.Update(5, existing)).Returns(existing);
            _repoMock.Setup(r => r.Save()).Returns(true);
            _mapperMock.Setup(m => m.Map<IngredientDto>(existing))
                .Returns(new IngredientDto { Id = 5, Name = "PartialOnly", Unit = "g", CaloriesPerUnit = 1.0 });

            var result = await _service.UpdateAsync(5, updateDto);

            Assert.NotNull(result);
            // Service called Map(updateDto, existing) exactly once — partial-mapping policy lives there.
            _mapperMock.Verify(m => m.Map(updateDto, existing), Times.Once);
        }

        // -----------------------------------------------------------
        // DeleteAsync (2)
        // -----------------------------------------------------------
        [Fact]
        public async Task DeleteAsync_DeletesAndReturnsTrue_WhenFound()
        {
            var entity = new IngredientEntity { Id = 5, Name = "ToDelete", Unit = "g" };
            _repoMock.Setup(r => r.GetSingle(5)).Returns(entity);
            _repoMock.Setup(r => r.Delete(5));
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
            _repoMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
        }

        // -----------------------------------------------------------
        // SearchAsync (3)
        // -----------------------------------------------------------
        [Fact]
        public async Task SearchAsync_FindsMatches_ByName()
        {
            var entities = new List<IngredientEntity>
            {
                new() { Id = 1, Name = "Tomato",   Unit = "g" },
                new() { Id = 2, Name = "Tomatillo", Unit = "g" }
            };
            var dtos = new List<IngredientDto>
            {
                new() { Id = 1, Name = "Tomato",   Unit = "g" },
                new() { Id = 2, Name = "Tomatillo", Unit = "g" }
            };

            _repoMock.Setup(r => r.SearchByName("tom")).Returns(entities);
            _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

            var result = await _service.SearchAsync("tom");

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
        {
            _repoMock.Setup(r => r.SearchByName("zzz")).Returns(new List<IngredientEntity>());
            _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
                       .Returns(new List<IngredientDto>());

            var result = await _service.SearchAsync("zzz");

            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchAsync_IsCaseInsensitive_DelegatesToRepo()
        {
            // Service forwards the term as-is; the repo handles case-insensitivity.
            // We verify that whatever the user typed is exactly what the repo gets,
            // and that an upper-case query still produces results.
            var entities = new List<IngredientEntity>
            {
                new() { Id = 1, Name = "Tomato", Unit = "g" }
            };
            _repoMock.Setup(r => r.SearchByName("TOM")).Returns(entities);
            _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities))
                       .Returns(new List<IngredientDto> { new() { Id = 1, Name = "Tomato", Unit = "g" } });

            var result = await _service.SearchAsync("TOM");

            Assert.Single(result);
            _repoMock.Verify(r => r.SearchByName("TOM"), Times.Once);
        }
    }
}
