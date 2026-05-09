using Moq;
using AutoMapper;
using MyFood.Application.Services;
using MyFood.Application.Dtos;
using MyFood.Application;
using MyFood.Domain.Entities;

public class IngredientServiceTests
{
    private readonly Mock<IIngredientRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IngredientService _service;

    public IngredientServiceTests()
    {
        _repoMock = new Mock<IIngredientRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new IngredientService(_repoMock.Object, _mapperMock.Object);
    }

    #region GetAllAsync Tests
    [Fact]
    public async Task GetAllAsync_ReturnsAllIngredients()
    {
        // Arrange
        var entities = new List<IngredientEntity>
        {
            new IngredientEntity { Id = 1, Name = "Tomato", Unit = "g", CaloriesPerUnit = 0.18m },
            new IngredientEntity { Id = 2, Name = "Onion", Unit = "g", CaloriesPerUnit = 0.4m }
        };
        var dtos = new List<IngredientDto>
        {
            new IngredientDto { Id = 1, Name = "Tomato", Unit = "g", CaloriesPerUnit = 0.18m },
            new IngredientDto { Id = 2, Name = "Onion", Unit = "g", CaloriesPerUnit = 0.4m }
        };

        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        // Act
        var result = await _service.GetAllAsync(new QueryParameters());

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal("Tomato", resultList[0].Name);
        Assert.Equal("Onion", resultList[1].Name);
        _repoMock.Verify(r => r.GetAll(It.IsAny<QueryParameters>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoIngredients()
    {
        // Arrange
        var entities = new List<IngredientEntity>();
        var dtos = new List<IngredientDto>();

        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_SupportsPagination()
    {
        // Arrange
        var entities = new List<IngredientEntity>
        {
            new IngredientEntity { Id = 1, Name = "Ingredient1" }
        };
        var dtos = new List<IngredientDto>
        {
            new IngredientDto { Id = 1, Name = "Ingredient1" }
        };

        var queryParams = new QueryParameters { Page = 2, PageCount = 10 };
        _repoMock.Setup(r => r.GetAll(queryParams)).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        // Act
        var result = await _service.GetAllAsync(queryParams);

        // Assert
        _repoMock.Verify(r => r.GetAll(queryParams), Times.Once);
        Assert.Single(result);
    }
    #endregion

    #region GetByIdAsync Tests
    [Fact]
    public async Task GetByIdAsync_ReturnsIngredient_WhenFound()
    {
        // Arrange
        var entity = new IngredientEntity { Id = 1, Name = "Salt", Unit = "g", CaloriesPerUnit = 0m };
        var dto = new IngredientDto { Id = 1, Name = "Salt", Unit = "g", CaloriesPerUnit = 0m };

        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Salt", result.Name);
        _repoMock.Verify(r => r.GetSingle(1), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

        // Act
        var result = await _service.GetByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WithNegativeId()
    {
        // Act
        var result = await _service.GetByIdAsync(-1);

        // Assert
        Assert.Null(result);
        _repoMock.Verify(r => r.GetSingle(It.IsAny<int>()), Times.Never);
    }
    #endregion

    #region CreateAsync Tests
    [Fact]
    public async Task CreateAsync_CreatesAndReturnsIngredient()
    {
        // Arrange
        var createDto = new IngredientCreateDto { Name = "Pepper", Unit = "g", CaloriesPerUnit = 3.2m, Protein = 0.1m, Carbs = 0.6m, Fat = 0.3m };
        var entity = new IngredientEntity { Name = "Pepper", Unit = "g", CaloriesPerUnit = 3.2m, Protein = 0.1m, Carbs = 0.6m, Fat = 0.3m };
        var createdEntity = new IngredientEntity { Id = 1, Name = "Pepper", Unit = "g", CaloriesPerUnit = 3.2m, Protein = 0.1m, Carbs = 0.6m, Fat = 0.3m };
        var resultDto = new IngredientDto { Id = 1, Name = "Pepper", Unit = "g", CaloriesPerUnit = 3.2m, Protein = 0.1m, Carbs = 0.6m, Fat = 0.3m };

        _repoMock.Setup(r => r.GetByNameAsync("Pepper")).ReturnsAsync((IngredientEntity?)null);
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.CreateAsync(entity)).ReturnsAsync(createdEntity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(createdEntity)).Returns(resultDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Pepper", result.Name);
        Assert.Equal(1, result.Id);
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<IngredientEntity>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_WhenNameIsNull()
    {
        // Arrange
        var createDto = new IngredientCreateDto { Name = null!, Unit = "g", CaloriesPerUnit = 1m };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_WhenNameIsEmpty()
    {
        // Arrange
        var createDto = new IngredientCreateDto { Name = "", Unit = "g", CaloriesPerUnit = 1m };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_WhenUnitIsNull()
    {
        // Arrange
        var createDto = new IngredientCreateDto { Name = "Garlic", Unit = null!, CaloriesPerUnit = 1m };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_WhenDuplicateName()
    {
        // Arrange
        var createDto = new IngredientCreateDto { Name = "Garlic", Unit = "g", CaloriesPerUnit = 1.4m };
        var existingEntity = new IngredientEntity { Id = 1, Name = "Garlic", Unit = "g" };

        _repoMock.Setup(r => r.GetByNameAsync("Garlic")).ReturnsAsync(existingEntity);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_MapsCorrectly()
    {
        // Arrange
        var createDto = new IngredientCreateDto { Name = "Basil", Unit = "g", CaloriesPerUnit = 2.3m, Protein = 0.3m, Carbs = 0.4m, Fat = 0.6m };
        var entity = new IngredientEntity { Name = "Basil", Unit = "g", CaloriesPerUnit = 2.3m, Protein = 0.3m, Carbs = 0.4m, Fat = 0.6m };
        var createdEntity = new IngredientEntity { Id = 5, Name = "Basil", Unit = "g", CaloriesPerUnit = 2.3m };
        var resultDto = new IngredientDto { Id = 5, Name = "Basil", Unit = "g", CaloriesPerUnit = 2.3m };

        _repoMock.Setup(r => r.GetByNameAsync("Basil")).ReturnsAsync((IngredientEntity?)null);
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.CreateAsync(entity)).ReturnsAsync(createdEntity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(createdEntity)).Returns(resultDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        _mapperMock.Verify(m => m.Map<IngredientEntity>(createDto), Times.Once);
        _mapperMock.Verify(m => m.Map<IngredientDto>(createdEntity), Times.Once);
    }
    #endregion

    #region UpdateAsync Tests
    [Fact]
    public async Task UpdateAsync_UpdatesAndReturnsIngredient()
    {
        // Arrange
        var updateDto = new IngredientUpdateDto { Name = "Sweet Pepper", Unit = "g", CaloriesPerUnit = 3.5m };
        var entity = new IngredientEntity { Id = 1, Name = "Pepper", Unit = "g", CaloriesPerUnit = 3.2m };
        var updatedEntity = new IngredientEntity { Id = 1, Name = "Sweet Pepper", Unit = "g", CaloriesPerUnit = 3.5m };
        var resultDto = new IngredientDto { Id = 1, Name = "Sweet Pepper", Unit = "g", CaloriesPerUnit = 3.5m };

        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _repoMock.Setup(r => r.GetByNameAsync("Sweet Pepper")).ReturnsAsync((IngredientEntity?)null);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<IngredientEntity>())).ReturnsAsync(updatedEntity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(updatedEntity)).Returns(resultDto);

        // Act
        var result = await _service.UpdateAsync(1, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Sweet Pepper", result.Name);
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<IngredientEntity>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsException_WhenNotFound()
    {
        // Arrange
        var updateDto = new IngredientUpdateDto { Name = "NewName" };
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(99, updateDto));
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate_OnlyUpdatesProvidedFields()
    {
        // Arrange
        var updateDto = new IngredientUpdateDto { Name = "Updated Tomato" };
        var entity = new IngredientEntity { Id = 1, Name = "Tomato", Unit = "g", CaloriesPerUnit = 0.18m, Protein = 0.08m, Carbs = 0.03m, Fat = 0.02m };
        var updatedEntity = new IngredientEntity { Id = 1, Name = "Updated Tomato", Unit = "g", CaloriesPerUnit = 0.18m, Protein = 0.08m, Carbs = 0.03m, Fat = 0.02m };
        var resultDto = new IngredientDto { Id = 1, Name = "Updated Tomato" };

        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _repoMock.Setup(r => r.GetByNameAsync("Updated Tomato")).ReturnsAsync((IngredientEntity?)null);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<IngredientEntity>())).ReturnsAsync(updatedEntity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(updatedEntity)).Returns(resultDto);

        // Act
        var result = await _service.UpdateAsync(1, updateDto);

        // Assert
        Assert.Equal("Updated Tomato", result.Name);
        Assert.Equal("g", entity.Unit); // Unit should remain unchanged
        Assert.Equal(0.18m, entity.CaloriesPerUnit); // Calories should remain unchanged
    }
    #endregion

    #region DeleteAsync Tests
    [Fact]
    public async Task DeleteAsync_DeletesIngredient_WhenFound()
    {
        // Arrange
        _repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        Assert.True(result);
        _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        // Arrange
        _repoMock.Setup(r => r.DeleteAsync(99)).ReturnsAsync(false);

        // Act
        var result = await _service.DeleteAsync(99);

        // Assert
        Assert.False(result);
    }
    #endregion

    #region SearchAsync Tests
    [Fact]
    public async Task SearchAsync_FindsMatches()
    {
        // Arrange
        var searchTerm = "tomato";
        var entities = new List<IngredientEntity>
        {
            new IngredientEntity { Id = 1, Name = "Tomato", Unit = "g" },
            new IngredientEntity { Id = 2, Name = "Cherry Tomato", Unit = "g" }
        };
        var dtos = new List<IngredientDto>
        {
            new IngredientDto { Id = 1, Name = "Tomato" },
            new IngredientDto { Id = 2, Name = "Cherry Tomato" }
        };

        _repoMock.Setup(r => r.SearchAsync(searchTerm)).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

        // Act
        var result = await _service.SearchAsync(searchTerm);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        _repoMock.Verify(r => r.SearchAsync(searchTerm), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
    {
        // Arrange
        var searchTerm = "xyz";
        _repoMock.Setup(r => r.SearchAsync(searchTerm)).ReturnsAsync(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto>());

        // Act
        var result = await _service.SearchAsync(searchTerm);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_IsCaseInsensitive()
    {
        // Arrange
        var searchTerm = "GARLIC";
        var entities = new List<IngredientEntity>
        {
            new IngredientEntity { Id = 1, Name = "Garlic", Unit = "g" }
        };
        var dtos = new List<IngredientDto>
        {
            new IngredientDto { Id = 1, Name = "Garlic" }
        };

        _repoMock.Setup(r => r.SearchAsync(searchTerm)).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

        // Act
        var result = await _service.SearchAsync(searchTerm);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenSearchTermIsNull()
    {
        // Act
        var result = await _service.SearchAsync(null!);

        // Assert
        Assert.Empty(result);
        _repoMock.Verify(r => r.SearchAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenSearchTermIsEmpty()
    {
        // Act
        var result = await _service.SearchAsync("");

        // Assert
        Assert.Empty(result);
    }
    #endregion
}
