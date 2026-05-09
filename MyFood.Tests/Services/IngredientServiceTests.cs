using FluentAssertions;
using Moq;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Repositories;
using MyFood.Application.Services;
using MyFood.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MyFood.Tests.Services
{
    public class IngredientServiceTests
    {
        private readonly Mock<IIngredientRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IngredientService _service;

        public IngredientServiceTests()
        {
            _mockRepo = new Mock<IIngredientRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new IngredientService(_mockRepo.Object, _mockMapper.Object);
        }

        #region GetAllAsync Tests (3 tests)

        [Fact]
        public async Task GetAllAsync_ReturnsAllIngredients()
        {
            // Arrange
            var ingredients = new List<IngredientEntity>
            {
                new() { Id = 1, Name = "Tomato" },
                new() { Id = 2, Name = "Cheese" }
            };
            var expectedDtos = new List<IngredientDto>
            {
                new() { Id = 1, Name = "Tomato" },
                new() { Id = 2, Name = "Cheese" }
            };

            _mockRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(ingredients);
            _mockMapper.Setup(x => x.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
                .Returns(expectedDtos);

            var queryParams = new QueryParameters { Page = 1, PageCount = 10 };

            // Act
            var result = await _service.GetAllIngredientsAsync(queryParams);

            // Assert
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expectedDtos);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmpty_WhenNoIngredients()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<IngredientEntity>());
            _mockMapper.Setup(x => x.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
                .Returns(new List<IngredientDto>());

            var queryParams = new QueryParameters { Page = 1, PageCount = 10 };

            // Act
            var result = await _service.GetAllIngredientsAsync(queryParams);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_PaginationWorks()
        {
            // Arrange
            var ingredients = new List<IngredientEntity>();
            for (int i = 1; i <= 25; i++)
            {
                ingredients.Add(new() { Id = i, Name = $"Ingredient{i}" });
            }

            _mockRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(ingredients);
            _mockMapper.Setup(x => x.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
                .Returns((IEnumerable<IngredientEntity> entities) => 
                    entities.Select(e => new IngredientDto { Id = e.Id, Name = e.Name }));

            var queryParams = new QueryParameters { Page = 2, PageCount = 10 };

            // Act
            var result = await _service.GetAllIngredientsAsync(queryParams);

            // Assert
            result.Should().HaveCount(10);
            result.First().Id.Should().Be(11);
            result.Last().Id.Should().Be(20);
        }

        #endregion

        #region GetByIdAsync Tests (3 tests)

        [Fact]
        public async Task GetByIdAsync_ReturnsIngredient_WhenFound()
        {
            // Arrange
            var ingredient = new IngredientEntity { Id = 1, Name = "Tomato" };
            var expectedDto = new IngredientDto { Id = 1, Name = "Tomato" };

            _mockRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(ingredient);
            _mockMapper.Setup(x => x.Map<IngredientDto>(ingredient)).Returns(expectedDto);

            // Act
            var result = await _service.GetIngredientByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Name.Should().Be("Tomato");
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((IngredientEntity?)null);

            // Act
            var result = await _service.GetIngredientByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNegativeId()
        {
            // Act
            var result = await _service.GetIngredientByIdAsync(-1);

            // Assert
            result.Should().BeNull();
            _mockRepo.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        #endregion

        #region CreateAsync Tests (4 tests)

        [Fact]
        public async Task CreateAsync_Success_ReturnsCreatedIngredient()
        {
            // Arrange
            var createDto = new IngredientCreateDto { Name = "New Ingredient", Quantity = 10, FoodId = 1 };
            var entity = new IngredientEntity { Id = 0, Name = "New Ingredient", Quantity = 10, FoodId = 1 };
            var createdEntity = new IngredientEntity { Id = 1, Name = "New Ingredient", Quantity = 10, FoodId = 1 };
            var expectedDto = new IngredientDto { Id = 1, Name = "New Ingredient", Quantity = 10, FoodId = 1 };

            _mockMapper.Setup(x => x.Map<IngredientEntity>(createDto)).Returns(entity);
            _mockRepo.Setup(x => x.AddAsync(entity)).ReturnsAsync(createdEntity);
            _mockMapper.Setup(x => x.Map<IngredientDto>(createdEntity)).Returns(expectedDto);

            // Act
            var result = await _service.CreateIngredientAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("New Ingredient");
        }

        [Fact]
        public async Task CreateAsync_ThrowsException_WhenNameIsNull()
        {
            // Arrange
            var createDto = new IngredientCreateDto { Name = null!, Quantity = 10, FoodId = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreateIngredientAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_ThrowsException_WhenNameIsEmpty()
        {
            // Arrange
            var createDto = new IngredientCreateDto { Name = "", Quantity = 10, FoodId = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreateIngredientAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_MapsCorrectly()
        {
            // Arrange
            var createDto = new IngredientCreateDto { Name = "Test", Quantity = 5, FoodId = 2 };
            var entity = new IngredientEntity { Name = "Test", Quantity = 5, FoodId = 2 };
            
            _mockMapper.Setup(x => x.Map<IngredientEntity>(createDto)).Returns(entity);
            _mockRepo.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);
            _mockMapper.Setup(x => x.Map<IngredientDto>(It.IsAny<IngredientEntity>()))
                .Returns(new IngredientDto { Name = "Test", Quantity = 5, FoodId = 2 });

            // Act
            var result = await _service.CreateIngredientAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Test");
            result.Quantity.Should().Be(5);
        }

        #endregion

        #region UpdateAsync Tests (3 tests)

        [Fact]
        public async Task UpdateAsync_Success_ReturnsUpdatedIngredient()
        {
            // Arrange
            var updateDto = new IngredientUpdateDto { Name = "Updated Name" };
            var existingEntity = new IngredientEntity { Id = 1, Name = "Old Name" };
            var updatedEntity = new IngredientEntity { Id = 1, Name = "Updated Name" };
            var expectedDto = new IngredientDto { Id = 1, Name = "Updated Name" };

            _mockRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingEntity);
            _mockMapper.Setup(x => x.Map(updateDto, existingEntity));
            _mockRepo.Setup(x => x.UpdateAsync(1, existingEntity)).ReturnsAsync(updatedEntity);
            _mockMapper.Setup(x => x.Map<IngredientDto>(updatedEntity)).Returns(expectedDto);

            // Act
            var result = await _service.UpdateIngredientAsync(1, updateDto);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Updated Name");
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            var updateDto = new IngredientUpdateDto { Name = "Updated" };
            _mockRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((IngredientEntity?)null);

            // Act
            var result = await _service.UpdateIngredientAsync(999, updateDto);

            // Assert
            result.Should().BeNull();
            _mockRepo.Verify(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<IngredientEntity>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_PartialUpdate_OnlyUpdatesProvidedFields()
        {
            // Arrange
            var updateDto = new IngredientUpdateDto { Name = "New Name Only", Quantity = null, FoodId = null };
            var existingEntity = new IngredientEntity { Id = 1, Name = "Old Name", Quantity = 10, FoodId = 5 };
            
            _mockRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingEntity);
            _mockRepo.Setup(x => x.UpdateAsync(1, It.IsAny<IngredientEntity>()))
                .ReturnsAsync((int id, IngredientEntity e) => e);
            _mockMapper.Setup(x => x.Map<IngredientDto>(It.IsAny<IngredientEntity>()))
                .Returns(new IngredientDto());

            // Act
            await _service.UpdateIngredientAsync(1, updateDto);

            // Assert
            _mockMapper.Verify(x => x.Map(updateDto, existingEntity), Times.Once);
        }

        #endregion

        #region DeleteAsync Tests (2 tests)

        [Fact]
        public async Task DeleteAsync_Success_ReturnsTrue()
        {
            // Arrange
            _mockRepo.Setup(x => x.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteIngredientAsync(1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
        {
            // Arrange
            _mockRepo.Setup(x => x.DeleteAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteIngredientAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region SearchAsync Tests (3 tests)

        [Fact]
        public async Task SearchAsync_FindsMatches()
        {
            // Arrange
            var ingredients = new List<IngredientEntity>
            {
                new() { Id = 1, Name = "Tomato" },
                new() { Id = 2, Name = "Cheese" },
                new() { Id = 3, Name = "Tofu" }
            };
            var expectedDtos = new List<IngredientDto>
            {
                new() { Id = 1, Name = "Tomato" },
                new() { Id = 3, Name = "Tofu" }
            };

            _mockRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(ingredients);
            _mockMapper.Setup(x => x.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
                .Returns(expectedDtos);

            // Act
            var result = await _service.SearchIngredientsByNameAsync("to");

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
        {
            // Arrange
            var ingredients = new List<IngredientEntity>
            {
                new() { Id = 1, Name = "Tomato" },
                new() { Id = 2, Name = "Cheese" }
            };

            _mockRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(ingredients);
            _mockMapper.Setup(x => x.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
                .Returns(new List<IngredientDto>());

            // Act
            var result = await _service.SearchIngredientsByNameAsync("xyz");

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task SearchAsync_CaseInsensitive()
        {
            // Arrange
            var ingredients = new List<IngredientEntity>
            {
                new() { Id = 1, Name = "TOMATO" }
            };
            var expectedDtos = new List<IngredientDto>
            {
                new() { Id = 1, Name = "TOMATO" }
            };

            _mockRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(ingredients);
            _mockMapper.Setup(x => x.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
                .Returns(expectedDtos);

            // Act
            var result = await _service.SearchIngredientsByNameAsync("tomato");

            // Assert
            result.Should().HaveCount(1);
        }

        #endregion
    }
}