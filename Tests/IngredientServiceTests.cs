using Moq;
using AutoMapper;
using MyFood.Application.Services;
using MyFood.Application.Dtos;
using MyFood.Application;
using MyFood.Domain.Entities;

public class IngredientServiceTests
{
    private readonly Mock<IFoodRepository> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly FoodService _service;

    public IngredientServiceTests()
    {
        _service = new FoodService(_repoMock.Object, _mapperMock.Object);
    }

    // ── GetAllAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsAll()
    {
        var entities = new List<FoodEntity>
        {
            new() { Id = 1, Name = "Apple", Ingredients = new List<IngredientEntity> { new() { Id = 1, Name = "Sugar" } } },
            new() { Id = 2, Name = "Banana", Ingredients = new List<IngredientEntity> { new() { Id = 2, Name = "Starch" } } }
        };
        var dtos = new List<FoodDto>
        {
            new() { Id = 1, Name = "Apple" },
            new() { Id = 2, Name = "Banana" }
        };

        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<FoodDto>>(It.IsAny<IEnumerable<FoodEntity>>())).Returns(dtos);

        var result = await _service.GetAllFoodsAsync(new QueryParameters());

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoData()
    {
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(new List<FoodEntity>().AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<FoodDto>>(It.IsAny<IEnumerable<FoodEntity>>())).Returns(new List<FoodDto>());

        var result = await _service.GetAllFoodsAsync(new QueryParameters());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_PaginationWorks()
    {
        var entities = Enumerable.Range(1, 20)
            .Select(i => new FoodEntity { Id = i, Name = $"Food{i}" })
            .ToList();
        var dtos = entities.Take(5).Select(e => new FoodDto { Id = e.Id, Name = e.Name }).ToList();

        var queryParams = new QueryParameters { Page = 1, PageCount = 5 };
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<FoodDto>>(It.IsAny<IEnumerable<FoodEntity>>())).Returns(dtos);

        var result = await _service.GetAllFoodsAsync(queryParams);

        Assert.Equal(5, result.Count());
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_Found()
    {
        var entity = new FoodEntity { Id = 1, Name = "Apple" };
        var dto = new FoodDto { Id = 1, Name = "Apple" };

        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _mapperMock.Setup(m => m.Map<FoodDto>(entity)).Returns(dto);

        var result = await _service.GetFoodByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Apple", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((FoodEntity)null!);

        var result = await _service.GetFoodByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_HandlesNegativeId()
    {
        _repoMock.Setup(r => r.GetSingle(-1)).Returns((FoodEntity)null!);

        var result = await _service.GetFoodByIdAsync(-1);

        Assert.Null(result);
    }

    // ── CreateAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_Success()
    {
        var createDto = new FoodCreateDto { Name = "Pear", Type = "Fruit", Calories = 100 };
        var entity = new FoodEntity { Id = 3, Name = "Pear" };
        var dto = new FoodDto { Id = 3, Name = "Pear" };

        _mapperMock.Setup(m => m.Map<FoodEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Add(entity));
        _repoMock.Setup(r => r.Save()).Returns(true);
        _repoMock.Setup(r => r.GetSingle(entity.Id)).Returns(entity);
        _mapperMock.Setup(m => m.Map<FoodDto>(entity)).Returns(dto);

        var result = await _service.CreateFoodAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal("Pear", result.Name);
    }

    [Fact]
public async Task CreateAsync_NullNameError()
{
    var createDto = new FoodCreateDto { Name = null!, Type = "Fruit", Calories = 100 };
    var entity = new FoodEntity { Id = 4, Name = null! };

    _mapperMock.Setup(m => m.Map<FoodEntity>(createDto)).Returns(entity);
    _repoMock.Setup(r => r.Add(entity));
    _repoMock.Setup(r => r.Save()).Returns(false);

    await Assert.ThrowsAsync<Exception>(() => _service.CreateFoodAsync(createDto));
}

[Fact]
public async Task CreateAsync_DuplicateError()
{
    var createDto = new FoodCreateDto { Name = "Apple", Type = "Fruit", Calories = 52 };
    var entity = new FoodEntity { Id = 5, Name = "Apple" };

    _mapperMock.Setup(m => m.Map<FoodEntity>(createDto)).Returns(entity);
    _repoMock.Setup(r => r.Add(entity));
    _repoMock.Setup(r => r.Save()).Returns(false);

    await Assert.ThrowsAsync<Exception>(() => _service.CreateFoodAsync(createDto));
}

    [Fact]
    public async Task CreateAsync_MapsCorrectly()
    {
        var createDto = new FoodCreateDto { Name = "Mango", Type = "Fruit", Calories = 60 };
        var entity = new FoodEntity { Id = 6, Name = "Mango", Type = "Fruit", Calories = 60 };
        var dto = new FoodDto { Id = 6, Name = "Mango", Type = "Fruit", Calories = 60 };

        _mapperMock.Setup(m => m.Map<FoodEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Add(entity));
        _repoMock.Setup(r => r.Save()).Returns(true);
        _repoMock.Setup(r => r.GetSingle(6)).Returns(entity);
        _mapperMock.Setup(m => m.Map<FoodDto>(entity)).Returns(dto);

        var result = await _service.CreateFoodAsync(createDto);

        Assert.Equal("Mango", result!.Name);
        Assert.Equal("Fruit", result.Type);
        Assert.Equal(60, result.Calories);
    }

    // ── UpdateAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_Success()
    {
        var updateDto = new FoodUpdateDto { Name = "Updated Apple" };
        var entity = new FoodEntity { Id = 1, Name = "Apple" };
        var updatedEntity = new FoodEntity { Id = 1, Name = "Updated Apple" };
        var dto = new FoodDto { Id = 1, Name = "Updated Apple" };

        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _mapperMock.Setup(m => m.Map(updateDto, entity));
        _repoMock.Setup(r => r.Update(1, entity)).Returns(updatedEntity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<FoodDto>(updatedEntity)).Returns(dto);

        var result = await _service.UpdateFoodAsync(1, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Updated Apple", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_NotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((FoodEntity)null!);

        var result = await _service.UpdateFoodAsync(99, new FoodUpdateDto());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate()
    {
        var updateDto = new FoodUpdateDto { Name = "Partial" };
        var entity = new FoodEntity { Id = 2, Name = "Old", Type = "Fruit", Calories = 50 };
        var updatedEntity = new FoodEntity { Id = 2, Name = "Partial", Type = "Fruit", Calories = 50 };
        var dto = new FoodDto { Id = 2, Name = "Partial", Type = "Fruit", Calories = 50 };

        _repoMock.Setup(r => r.GetSingle(2)).Returns(entity);
        _mapperMock.Setup(m => m.Map(updateDto, entity));
        _repoMock.Setup(r => r.Update(2, entity)).Returns(updatedEntity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<FoodDto>(updatedEntity)).Returns(dto);

        var result = await _service.UpdateFoodAsync(2, updateDto);

        Assert.Equal("Partial", result!.Name);
        Assert.Equal("Fruit", result.Type);
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_Success()
    {
        var entity = new FoodEntity { Id = 1 };
        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _repoMock.Setup(r => r.Delete(1));
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.DeleteFoodAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_NotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((FoodEntity)null!);

        var result = await _service.DeleteFoodAsync(99);

        Assert.False(result);
    }

    // ── SearchAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task SearchAsync_FindsMatches()
    {
        var entities = new List<FoodEntity>
        {
            new() { Id = 1, Name = "Apple Juice" },
            new() { Id = 2, Name = "Apple Pie" }
        };
        var dtos = new List<FoodDto>
        {
            new() { Id = 1, Name = "Apple Juice" },
            new() { Id = 2, Name = "Apple Pie" }
        };

        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<FoodDto>>(It.IsAny<IEnumerable<FoodEntity>>())).Returns(dtos);

        var result = await _service.GetAllFoodsAsync(new QueryParameters());

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatch()
    {
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(new List<FoodEntity>().AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<FoodDto>>(It.IsAny<IEnumerable<FoodEntity>>())).Returns(new List<FoodDto>());

        var result = await _service.GetAllFoodsAsync(new QueryParameters());

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_CaseInsensitive()
    {
        var entities = new List<FoodEntity> { new() { Id = 1, Name = "apple" } };
        var dtos = new List<FoodDto> { new() { Id = 1, Name = "apple" } };

        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<FoodDto>>(It.IsAny<IEnumerable<FoodEntity>>())).Returns(dtos);

        var result = await _service.GetAllFoodsAsync(new QueryParameters());

        Assert.Single(result);
        Assert.Equal("apple", result.First().Name);
    }
}