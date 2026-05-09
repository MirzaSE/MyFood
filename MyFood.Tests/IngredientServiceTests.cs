using AutoMapper;
using Moq;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

public class IngredientServiceTests
{
    private readonly Mock<IIngredientRepository> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly IngredientService _service;

    public IngredientServiceTests()
    {
        _service = new IngredientService(_repoMock.Object, _mapperMock.Object);
    }

    // --- GetAllAsync ---

    [Fact]
    public async Task GetAllAsync_ReturnsAllIngredients()
    {
        var entities = new List<IngredientEntity> { new() { Id = 1, Name = "Salt" }, new() { Id = 2, Name = "Pepper" } };
        var dtos = new List<IngredientDto> { new() { Id = 1, Name = "Salt" }, new() { Id = 2, Name = "Pepper" } };
        var qp = new QueryParameters();

        _repoMock.Setup(r => r.GetAllAsync(qp)).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

        var result = await _service.GetAllAsync(qp);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoIngredients()
    {
        var qp = new QueryParameters();
        _repoMock.Setup(r => r.GetAllAsync(qp)).ReturnsAsync(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<List<IngredientEntity>>())).Returns(new List<IngredientDto>());

        var result = await _service.GetAllAsync(qp);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_PaginationWorks()
    {
        var qp = new QueryParameters { Page = 2, PageCount = 5 };
        var entities = new List<IngredientEntity> { new() { Id = 6, Name = "Sugar" } };
        var dtos = new List<IngredientDto> { new() { Id = 6, Name = "Sugar" } };

        _repoMock.Setup(r => r.GetAllAsync(qp)).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

        var result = await _service.GetAllAsync(qp);

        Assert.Single(result);
        Assert.Equal("Sugar", result.First().Name);
    }

    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenFound()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Salt" };
        var dto = new IngredientDto { Id = 1, Name = "Salt" };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Salt", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((IngredientEntity?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_ForNegativeId()
    {
        var result = await _service.GetByIdAsync(-1);

        Assert.Null(result);
        _repoMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    // --- CreateAsync ---

    [Fact]
    public async Task CreateAsync_ReturnsDto_OnSuccess()
    {
        var createDto = new IngredientCreateDto { Name = "Olive Oil", Unit = "ml", CaloriesPerUnit = 9 };
        var entity = new IngredientEntity { Id = 1, Name = "Olive Oil" };
        var dto = new IngredientDto { Id = 1, Name = "Olive Oil" };

        _repoMock.Setup(r => r.ExistsAsync("Olive Oil")).ReturnsAsync(false);
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.CreateAsync(createDto);

        Assert.Equal("Olive Oil", result.Name);
    }

    [Fact]
    public async Task CreateAsync_ThrowsArgumentException_WhenNameIsNull()
    {
        var createDto = new IngredientCreateDto { Name = "" };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsInvalidOperationException_WhenDuplicate()
    {
        var createDto = new IngredientCreateDto { Name = "Salt" };
        _repoMock.Setup(r => r.ExistsAsync("Salt")).ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_MapsCorrectly()
    {
        var createDto = new IngredientCreateDto { Name = "Butter", Unit = "g", CaloriesPerUnit = 7.2, Protein = 0.1, Carbs = 0, Fat = 0.8 };
        var entity = new IngredientEntity { Id = 5, Name = "Butter", Unit = "g", CaloriesPerUnit = 7.2 };
        var dto = new IngredientDto { Id = 5, Name = "Butter", Unit = "g", CaloriesPerUnit = 7.2 };

        _repoMock.Setup(r => r.ExistsAsync("Butter")).ReturnsAsync(false);
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.CreateAsync(createDto);

        Assert.Equal("Butter", result.Name);
        Assert.Equal("g", result.Unit);
        Assert.Equal(7.2, result.CaloriesPerUnit);
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_ReturnsUpdatedDto_OnSuccess()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Salt", Unit = "g" };
        var updateDto = new IngredientUpdateDto { Name = "Sea Salt" };
        var updatedEntity = new IngredientEntity { Id = 1, Name = "Sea Salt", Unit = "g" };
        var dto = new IngredientDto { Id = 1, Name = "Sea Salt" };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _repoMock.Setup(r => r.UpdateAsync(1, It.IsAny<IngredientEntity>())).ReturnsAsync(updatedEntity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(updatedEntity)).Returns(dto);

        var result = await _service.UpdateAsync(1, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Sea Salt", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((IngredientEntity?)null);

        var result = await _service.UpdateAsync(99, new IngredientUpdateDto());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_OnlyUpdatesProvidedFields()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Salt", Unit = "g", CaloriesPerUnit = 0 };
        var updateDto = new IngredientUpdateDto { CaloriesPerUnit = 5.5 };
        var updatedEntity = new IngredientEntity { Id = 1, Name = "Salt", Unit = "g", CaloriesPerUnit = 5.5 };
        var dto = new IngredientDto { Id = 1, Name = "Salt", CaloriesPerUnit = 5.5 };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _repoMock.Setup(r => r.UpdateAsync(1, It.IsAny<IngredientEntity>())).ReturnsAsync(updatedEntity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(updatedEntity)).Returns(dto);

        var result = await _service.UpdateAsync(1, updateDto);

        Assert.Equal("Salt", result!.Name);
        Assert.Equal(5.5, result.CaloriesPerUnit);
    }

    // --- DeleteAsync ---

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_OnSuccess()
    {
        _repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        _repoMock.Setup(r => r.DeleteAsync(99)).ReturnsAsync(false);

        var result = await _service.DeleteAsync(99);

        Assert.False(result);
    }

    // --- SearchAsync ---

    [Fact]
    public async Task SearchAsync_ReturnsMatches()
    {
        var entities = new List<IngredientEntity> { new() { Id = 1, Name = "Salt" } };
        var dtos = new List<IngredientDto> { new() { Id = 1, Name = "Salt" } };

        _repoMock.Setup(r => r.SearchAsync("salt")).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

        var result = await _service.SearchAsync("salt");

        Assert.Single(result);
        Assert.Equal("Salt", result.First().Name);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
    {
        _repoMock.Setup(r => r.SearchAsync("xyz")).ReturnsAsync(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<List<IngredientEntity>>())).Returns(new List<IngredientDto>());

        var result = await _service.SearchAsync("xyz");

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_IsCaseInsensitive()
    {
        var entities = new List<IngredientEntity> { new() { Id = 1, Name = "Sugar" } };
        var dtos = new List<IngredientDto> { new() { Id = 1, Name = "Sugar" } };

        _repoMock.Setup(r => r.SearchAsync("SUGAR")).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

        var result = await _service.SearchAsync("SUGAR");

        Assert.Single(result);
        Assert.Equal("Sugar", result.First().Name);
    }
}
