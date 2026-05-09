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

        // ---------- GetAllAsync ----------

        [Fact]
        public async Task GetAllAsync_ReturnsAll_WhenIngredientsExist()
        {
            var entities = new List<IngredientEntity>
            {
                new() { Id = 1, Name = "Tomato", Unit = "g", CaloriesPerUnit = 0.18m },
                new() { Id = 2, Name = "Onion",  Unit = "g", CaloriesPerUnit = 0.40m }
            };
            var dtos = new List<IngredientDto>
            {
                new() { Id = 1, Name = "Tomato" },
                new() { Id = 2, Name = "Onion" }
            };

            _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
            _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

            var result = await _service.GetAllAsync(new QueryParameters());

            Assert.Equal(2, result.Count());
            Assert.Contains(result, i => i.Name == "Tomato");
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmpty_WhenNoIngredients()
        {
            _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(new List<IngredientEntity>().AsQueryable());
            _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
                       .Returns(new List<IngredientDto>());

            var result = await _service.GetAllAsync(new QueryParameters());

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_PaginationWorks_PassesQueryParametersToRepo()
        {
            var queryParams = new QueryParameters { Page = 2, PageCount = 5 };
            var entities = new List<IngredientEntity>
            {
                new() { Id = 6, Name = "Garlic" }
            };
            var dtos = new List<IngredientDto> { new() { Id = 6, Name = "Garlic" } };

            _repoMock.Setup(r => r.GetAll(It.Is<QueryParameters>(q => q.Page == 2 && q.PageCount == 5)))
                     .Returns(entities.AsQueryable());
            _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

            var result = await _service.GetAllAsync(queryParams);

            _repoMock.Verify(r => r.GetAll(It.Is<QueryParameters>(q => q.Page == 2 && q.PageCount == 5)), Times.Once);
            Assert.Single(result);
        }

        // ---------- GetByIdAsync ----------

        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenFound()
        {
            var entity = new IngredientEntity { Id = 1, Name = "Salt" };
            var dto = new IngredientDto { Id = 1, Name = "Salt" };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Salt", result!.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((IngredientEntity?)null);

            var result = await _service.GetByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_Throws_WhenIdIsNegative()
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.GetByIdAsync(-1));
            _repoMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        // ---------- CreateAsync ----------

        [Fact]
        public async Task CreateAsync_ReturnsDto_OnSuccess()
        {
            var createDto = new IngredientCreateDto { Name = "Pepper", Unit = "g", CaloriesPerUnit = 2.5m };
            var entity = new IngredientEntity { Id = 0, Name = "Pepper" };
            var savedEntity = new IngredientEntity { Id = 5, Name = "Pepper" };
            var resultDto = new IngredientDto { Id = 5, Name = "Pepper" };

            _repoMock.Setup(r => r.ExistsByNameAsync("Pepper", null)).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
            _repoMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(savedEntity);
            _mapperMock.Setup(m => m.Map<IngredientDto>(savedEntity)).Returns(resultDto);

            var result = await _service.CreateAsync(createDto);

            Assert.Equal(5, result.Id);
            Assert.Equal("Pepper", result.Name);
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenNameIsNullOrEmpty()
        {
            var createDto = new IngredientCreateDto { Name = "", Unit = "g", CaloriesPerUnit = 1m };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
            _repoMock.Verify(r => r.AddAsync(It.IsAny<IngredientEntity>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenIngredientNameAlreadyExists()
        {
            var createDto = new IngredientCreateDto { Name = "Tomato", Unit = "g", CaloriesPerUnit = 0.18m };
            _repoMock.Setup(r => r.ExistsByNameAsync("Tomato", null)).ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
            _repoMock.Verify(r => r.AddAsync(It.IsAny<IngredientEntity>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_MapsDtoToEntityCorrectly()
        {
            var createDto = new IngredientCreateDto
            {
                Name = "Olive Oil",
                Unit = "ml",
                CaloriesPerUnit = 8.84m,
                Protein = 0,
                Carbs = 0,
                Fat = 1
            };
            var entity = new IngredientEntity { Name = "Olive Oil", Unit = "ml" };
            var savedEntity = new IngredientEntity { Id = 7, Name = "Olive Oil", Unit = "ml" };
            var resultDto = new IngredientDto { Id = 7, Name = "Olive Oil", Unit = "ml" };

            _repoMock.Setup(r => r.ExistsByNameAsync("Olive Oil", null)).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
            _repoMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(savedEntity);
            _mapperMock.Setup(m => m.Map<IngredientDto>(savedEntity)).Returns(resultDto);

            var result = await _service.CreateAsync(createDto);

            _mapperMock.Verify(m => m.Map<IngredientEntity>(createDto), Times.Once);
            _mapperMock.Verify(m => m.Map<IngredientDto>(savedEntity), Times.Once);
            Assert.Equal("Olive Oil", result.Name);
            Assert.Equal("ml", result.Unit);
        }

        // ---------- UpdateAsync ----------

        [Fact]
        public async Task UpdateAsync_ReturnsUpdatedDto_OnSuccess()
        {
            var updateDto = new IngredientUpdateDto { Name = "Sea Salt", CaloriesPerUnit = 0m };
            var existing = new IngredientEntity { Id = 3, Name = "Salt" };
            var updated = new IngredientEntity { Id = 3, Name = "Sea Salt" };
            var resultDto = new IngredientDto { Id = 3, Name = "Sea Salt" };

            _repoMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(existing);
            _mapperMock.Setup(m => m.Map(updateDto, existing)).Returns(existing);
            _repoMock.Setup(r => r.UpdateAsync(3, existing)).ReturnsAsync(updated);
            _mapperMock.Setup(m => m.Map<IngredientDto>(updated)).Returns(resultDto);

            var result = await _service.UpdateAsync(3, updateDto);

            Assert.NotNull(result);
            Assert.Equal("Sea Salt", result!.Name);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((IngredientEntity?)null);

            var result = await _service.UpdateAsync(99, new IngredientUpdateDto { Name = "X" });

            Assert.Null(result);
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<int>(), It.IsAny<IngredientEntity>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_PartialUpdate_OnlyUpdatesProvidedFields()
        {
            // Only Name is provided; the other fields stay at their previous values.
            var updateDto = new IngredientUpdateDto { Name = "Updated Name" };
            var existing = new IngredientEntity
            {
                Id = 4,
                Name = "Old Name",
                Unit = "g",
                CaloriesPerUnit = 1.5m,
                Protein = 2m
            };
            var updated = new IngredientEntity
            {
                Id = 4,
                Name = "Updated Name",
                Unit = "g",
                CaloriesPerUnit = 1.5m,
                Protein = 2m
            };
            var resultDto = new IngredientDto { Id = 4, Name = "Updated Name", Unit = "g", CaloriesPerUnit = 1.5m, Protein = 2m };

            _repoMock.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(existing);
            _mapperMock.Setup(m => m.Map(updateDto, existing)).Returns(existing);
            _repoMock.Setup(r => r.UpdateAsync(4, existing)).ReturnsAsync(updated);
            _mapperMock.Setup(m => m.Map<IngredientDto>(updated)).Returns(resultDto);

            var result = await _service.UpdateAsync(4, updateDto);

            Assert.NotNull(result);
            Assert.Equal("Updated Name", result!.Name);
            Assert.Equal("g", result.Unit);                  // unchanged
            Assert.Equal(1.5m, result.CaloriesPerUnit);      // unchanged
            Assert.Equal(2m, result.Protein);                // unchanged
        }

        // ---------- DeleteAsync ----------

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_OnSuccess()
        {
            _repoMock.Setup(r => r.DeleteAsync(7)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(7);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
        {
            _repoMock.Setup(r => r.DeleteAsync(99)).ReturnsAsync(false);

            var result = await _service.DeleteAsync(99);

            Assert.False(result);
        }

        // ---------- SearchAsync ----------

        [Fact]
        public async Task SearchAsync_ReturnsMatches_WhenIngredientsMatchTerm()
        {
            var entities = new List<IngredientEntity>
            {
                new() { Id = 1, Name = "Tomato Paste" },
                new() { Id = 2, Name = "Cherry Tomato" }
            };
            var dtos = new List<IngredientDto>
            {
                new() { Id = 1, Name = "Tomato Paste" },
                new() { Id = 2, Name = "Cherry Tomato" }
            };

            _repoMock.Setup(r => r.SearchAsync("tomato")).ReturnsAsync(entities);
            _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

            var result = await _service.SearchAsync("tomato");

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
        {
            _repoMock.Setup(r => r.SearchAsync("xyz")).ReturnsAsync(new List<IngredientEntity>());
            _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
                       .Returns(new List<IngredientDto>());

            var result = await _service.SearchAsync("xyz");

            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchAsync_IsCaseInsensitive()
        {
            // Repo returns mixed-case results; service must filter case-insensitively.
            var entities = new List<IngredientEntity>
            {
                new() { Id = 1, Name = "TOMATO" },
                new() { Id = 2, Name = "tomato paste" },
                new() { Id = 3, Name = "Banana" } // should not match "tomato" search
            };

            _repoMock.Setup(r => r.SearchAsync(It.IsAny<string>())).ReturnsAsync(entities);
            _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
                       .Returns<IEnumerable<IngredientEntity>>(src =>
                            src.Select(e => new IngredientDto { Id = e.Id, Name = e.Name }).ToList());

            var result = await _service.SearchAsync("ToMaTo");

            Assert.Equal(2, result.Count());
            Assert.All(result, dto => Assert.Contains("tomato", dto.Name, StringComparison.OrdinalIgnoreCase));
            Assert.DoesNotContain(result, dto => dto.Name == "Banana");
        }
    }
}
