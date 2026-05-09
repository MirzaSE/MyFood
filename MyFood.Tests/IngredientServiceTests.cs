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

    // ── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsAllIngredients()
    {
        var entities = new List<IngredientEntity> { new() { Id = 1, Name = "Salt" } };
        var dtos = new List<IngridientDto> { new() { Id = 1, Name = "Salt" } };
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngridientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.GetAllAsync(new QueryParameters());

        Assert.Single(result);
        Assert.Equal("Salt", result.First().Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoIngredients()
    {
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(new List<IngredientEntity>().AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngridientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngridientDto>());

        var result = await _service.GetAllAsync(new QueryParameters());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_PaginationWorks()
    {
        var entities = new List<IngredientEntity>
        {
            new() { Id = 1, Name = "Salt" },
            new() { Id = 2, Name = "Sugar" }
        };
        var dtos = new List<IngridientDto> { new() { Id = 1, Name = "Salt" } };
        var queryParams = new QueryParameters { Page = 1, PageCount = 1 };

        _repoMock.Setup(r => r.GetAll(queryParams)).Returns(entities.Take(1).AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngridientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.GetAllAsync(queryParams);

        Assert.Single(result);
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenFound()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Pepper" };
        var dto = new IngridientDto { Id = 1, Name = "Pepper" };
        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngridientDto>(entity)).Returns(dto);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Pepper", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsException_ForNegativeId()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.GetByIdAsync(-1));
    }

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ReturnsDto_OnSuccess()
    {
        var createDto = new IngridientCreateDto { Name = "Basil", Quantity = 5, FoodId = 1 };
        var entity = new IngredientEntity { Id = 3, Name = "Basil" };
        var dto = new IngridientDto { Id = 3, Name = "Basil" };

        _repoMock.Setup(r => r.SearchFoodsByName("Basil")).Returns(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Add(entity));
        _repoMock.Setup(r => r.Save()).Returns(true);
        _repoMock.Setup(r => r.GetSingle(entity.Id)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngridientDto>(entity)).Returns(dto);

        var result = await _service.CreateAsync(createDto);

        Assert.Equal("Basil", result.Name);
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_WhenNameIsNull()
    {
        var createDto = new IngridientCreateDto { Name = "", Quantity = 1, FoodId = 1 };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_WhenDuplicate()
    {
        var createDto = new IngridientCreateDto { Name = "Salt", Quantity = 1, FoodId = 1 };
        var existing = new List<IngredientEntity> { new() { Id = 1, Name = "Salt" } };
        _repoMock.Setup(r => r.SearchFoodsByName("Salt")).Returns(existing);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_MapsCorrectly()
    {
        var createDto = new IngridientCreateDto { Name = "Thyme", Quantity = 2, FoodId = 1 };
        var entity = new IngredientEntity { Id = 5, Name = "Thyme" };
        var dto = new IngridientDto { Id = 5, Name = "Thyme" };

        _repoMock.Setup(r => r.SearchFoodsByName("Thyme")).Returns(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _repoMock.Setup(r => r.GetSingle(entity.Id)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngridientDto>(entity)).Returns(dto);

        await _service.CreateAsync(createDto);

        _mapperMock.Verify(m => m.Map<IngredientEntity>(createDto), Times.Once);
        _mapperMock.Verify(m => m.Map<IngridientDto>(entity), Times.Once);
    }

    // ── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ReturnsDto_OnSuccess()
    {
        var updateDto = new IngridientUpdate { Name = "Updated" };
        var entity = new IngredientEntity { Id = 4, Name = "Old" };
        var updatedEntity = new IngredientEntity { Id = 4, Name = "Updated" };
        var dto = new IngridientDto { Id = 4, Name = "Updated" };

        _repoMock.Setup(r => r.GetSingle(4)).Returns(entity);
        _mapperMock.Setup(m => m.Map(updateDto, entity));
        _repoMock.Setup(r => r.Update(4, entity)).Returns(updatedEntity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngridientDto>(updatedEntity)).Returns(dto);

        var result = await _service.UpdateAsync(4, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Updated", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

        var result = await _service.UpdateAsync(99, new IngridientUpdate());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate_OnlyUpdatesProvidedFields()
    {
        var updateDto = new IngridientUpdate { Name = "Partial" };
        var entity = new IngredientEntity { Id = 6, Name = "Original" };
        var updatedEntity = new IngredientEntity { Id = 6, Name = "Partial" };
        var dto = new IngridientDto { Id = 6, Name = "Partial" };

        _repoMock.Setup(r => r.GetSingle(6)).Returns(entity);
        _mapperMock.Setup(m => m.Map(updateDto, entity));
        _repoMock.Setup(r => r.Update(6, entity)).Returns(updatedEntity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngridientDto>(updatedEntity)).Returns(dto);

        var result = await _service.UpdateAsync(6, updateDto);

        _mapperMock.Verify(m => m.Map(updateDto, entity), Times.Once);
        Assert.Equal("Partial", result!.Name);
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenDeleted()
    {
        var entity = new IngredientEntity { Id = 7, Name = "Garlic" };
        _repoMock.Setup(r => r.GetSingle(7)).Returns(entity);
        _repoMock.Setup(r => r.Delete(7));
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.DeleteAsync(7);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

        var result = await _service.DeleteAsync(99);

        Assert.False(result);
    }

    // ── SearchAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task SearchAsync_ReturnsMatches_WhenFound()
    {
        var entities = new List<IngredientEntity> { new() { Id = 1, Name = "Cumin" } };
        var dtos = new List<IngridientDto> { new() { Id = 1, Name = "Cumin" } };
        _repoMock.Setup(r => r.SearchFoodsByName("cumin")).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngridientDto>>(entities)).Returns(dtos);

        var result = await _service.SearchAsync("cumin");

        Assert.Single(result);
        Assert.Equal("Cumin", result.First().Name);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
    {
        _repoMock.Setup(r => r.SearchFoodsByName("xyz")).Returns(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngridientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngridientDto>());

        var result = await _service.SearchAsync("xyz");

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_IsCaseInsensitive()
    {
        var entities = new List<IngredientEntity> { new() { Id = 1, Name = "Oregano" } };
        var dtos = new List<IngridientDto> { new() { Id = 1, Name = "Oregano" } };
        _repoMock.Setup(r => r.SearchFoodsByName("OREGANO")).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngridientDto>>(entities)).Returns(dtos);

        var result = await _service.SearchAsync("OREGANO");

        Assert.Single(result);
        Assert.Equal("Oregano", result.First().Name);
    }
}
