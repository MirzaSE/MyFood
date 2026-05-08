using Moq;
using AutoMapper;
using MyFood.Application.Services;
using MyFood.Application.Dtos;
using MyFood.Application;
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

    // GetAllAsync
    [Fact]
    public async Task GetAllAsync_ReturnsAllIngredients()
    {
        var entities = new List<IngredientEntity> { new() { Id = 1, Name = "Salt" } }.AsQueryable();
        var dtos = new List<IngredientDto> { new() { Id = 1, Name = "Salt" } };
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.GetAllAsync(new QueryParameters());
        Assert.Single(result);
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
    public async Task GetAllAsync_PaginationWorks()
    {
        var entities = new List<IngredientEntity> { new() { Id = 1, Name = "Salt" } }.AsQueryable();
        var dtos = new List<IngredientDto> { new() { Id = 1, Name = "Salt" } };
        var qp = new QueryParameters { Page = 1, PageCount = 5 };
        _repoMock.Setup(r => r.GetAll(qp)).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.GetAllAsync(qp);
        Assert.NotNull(result);
    }

    // GetByIdAsync
    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenFound()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Sugar" };
        var dto = new IngredientDto { Id = 1, Name = "Sugar" };
        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto?>(entity)).Returns(dto);

        var result = await _service.GetByIdAsync(1);
        Assert.Equal("Sugar", result!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity)null!);
        _mapperMock.Setup(m => m.Map<IngredientDto?>(null)).Returns((IngredientDto?)null);

        var result = await _service.GetByIdAsync(99);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_ForNegativeId()
    {
        var result = await _service.GetByIdAsync(-1);
        Assert.Null(result);
    }

    // CreateAsync
    [Fact]
    public async Task CreateAsync_ReturnsDto_OnSuccess()
    {
        var createDto = new IngredientCreateDto { Name = "Pepper", Unit = "g" };
        var entity = new IngredientEntity { Id = 1, Name = "Pepper" };
        var dto = new IngredientDto { Id = 1, Name = "Pepper" };
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.CreateAsync(createDto);
        Assert.Equal("Pepper", result.Name);
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_WhenNameIsNull()
    {
        var createDto = new IngredientCreateDto { Name = "", Unit = "g" };
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_WhenNameIsWhitespace()
    {
        var createDto = new IngredientCreateDto { Name = "   ", Unit = "g" };
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_MapsCorrectly()
    {
        var createDto = new IngredientCreateDto { Name = "Oil", Unit = "ml", CaloriesPerUnit = 9 };
        var entity = new IngredientEntity { Id = 2, Name = "Oil", Unit = "ml", CaloriesPerUnit = 9 };
        var dto = new IngredientDto { Id = 2, Name = "Oil", Unit = "ml", CaloriesPerUnit = 9 };
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.CreateAsync(createDto);
        Assert.Equal(9, result.CaloriesPerUnit);
    }

    // UpdateAsync
    [Fact]
    public async Task UpdateAsync_ReturnsUpdatedDto_OnSuccess()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Salt", Unit = "g" };
        var updateDto = new IngredientUpdateDto { Name = "Sea Salt" };
        var dto = new IngredientDto { Id = 1, Name = "Sea Salt" };
        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _repoMock.Setup(r => r.Update(1, entity)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.UpdateAsync(1, updateDto);
        Assert.Equal("Sea Salt", result!.Name);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity)null!);

        var result = await _service.UpdateAsync(99, new IngredientUpdateDto());
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate_OnlyChangesProvidedFields()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Salt", Unit = "g", Protein = 5 };
        var updateDto = new IngredientUpdateDto { Name = "Sea Salt" };
        var dto = new IngredientDto { Id = 1, Name = "Sea Salt", Unit = "g", Protein = 5 };
        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _repoMock.Setup(r => r.Update(1, entity)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.UpdateAsync(1, updateDto);
        Assert.Equal("g", result!.Unit);
        Assert.Equal(5, result.Protein);
    }

    // DeleteAsync
    [Fact]
    public async Task DeleteAsync_ReturnsTrue_OnSuccess()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Salt" };
        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.DeleteAsync(1);
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity)null!);

        var result = await _service.DeleteAsync(99);
        Assert.False(result);
    }

    // SearchAsync
    [Fact]
    public async Task SearchAsync_ReturnsMatchingIngredients()
    {
        var entities = new List<IngredientEntity> { new() { Id = 1, Name = "Salt" } };
        var dtos = new List<IngredientDto> { new() { Id = 1, Name = "Salt" } };
        _repoMock.Setup(r => r.SearchFoodsByName("salt")).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

        var result = await _service.SearchAsync("salt");
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatch()
    {
        _repoMock.Setup(r => r.SearchFoodsByName("xyz")).Returns(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto>());

        var result = await _service.SearchAsync("xyz");
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_IsCaseInsensitive()
    {
        var entities = new List<IngredientEntity> { new() { Id = 1, Name = "Salt" } };
        var dtos = new List<IngredientDto> { new() { Id = 1, Name = "Salt" } };
        _repoMock.Setup(r => r.SearchFoodsByName("SALT")).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

        var result = await _service.SearchAsync("SALT");
        Assert.Single(result);
    }
}