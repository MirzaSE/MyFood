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

    [Fact]
    public async Task GetAllAsync_ReturnsAllIngredients()
    {
        var query = new QueryParameters { Page = 1, PageCount = 10 };
        var entities = new List<IngredientEntity>
        {
            new() { Id = 1, Name = "Salt", Quantity = 1, FoodEntityId = 10 },
            new() { Id = 2, Name = "Pepper", Quantity = 2, FoodEntityId = 10 }
        };
        var dtos = new List<IngredientDto>
        {
            new() { Id = 1, Name = "Salt", Quantity = 1, FoodEntityId = 10 },
            new() { Id = 2, Name = "Pepper", Quantity = 2, FoodEntityId = 10 }
        };

        _repoMock.Setup(r => r.GetAll(query)).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.GetAllAsync(query);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoIngredients()
    {
        var query = new QueryParameters { Page = 1, PageCount = 10 };
        var entities = new List<IngredientEntity>();
        var dtos = new List<IngredientDto>();

        _repoMock.Setup(r => r.GetAll(query)).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.GetAllAsync(query);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_Pagination_Works()
    {
        var query = new QueryParameters { Page = 2, PageCount = 5 };
        var entities = new List<IngredientEntity>
        {
            new() { Id = 6, Name = "Sugar", Quantity = 1, FoodEntityId = 20 }
        };
        var dtos = new List<IngredientDto>
        {
            new() { Id = 6, Name = "Sugar", Quantity = 1, FoodEntityId = 20 }
        };

        _repoMock.Setup(r => r.GetAll(It.Is<QueryParameters>(q => q.Page == 2 && q.PageCount == 5)))
            .Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.GetAllAsync(query);

        Assert.Single(result);
        Assert.Equal(6, result.First().Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsIngredient_WhenFound()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Salt", Quantity = 1, FoodEntityId = 10 };
        var dto = new IngredientDto { Id = 1, Name = "Salt", Quantity = 1, FoodEntityId = 10 };

        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Salt", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_HandlesNegativeIds()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.GetByIdAsync(-1));
    }

    [Fact]
    public async Task CreateAsync_Success()
    {
        var createDto = new IngredientCreateDto { Name = "Flour", Quantity = 2, FoodEntityId = 1 };
        var entity = new IngredientEntity { Id = 5, Name = "Flour", Quantity = 2, FoodEntityId = 1 };
        var dto = new IngredientDto { Id = 5, Name = "Flour", Quantity = 2, FoodEntityId = 1 };

        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(new List<IngredientEntity>().AsQueryable());
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _repoMock.Setup(r => r.GetSingle(5)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.CreateAsync(createDto);

        Assert.Equal("Flour", result.Name);
        _repoMock.Verify(r => r.Add(entity), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_NullNameError()
    {
        var createDto = new IngredientCreateDto { Name = null, Quantity = 1, FoodEntityId = 1 };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_DuplicateError()
    {
        var createDto = new IngredientCreateDto { Name = "Salt", Quantity = 1, FoodEntityId = 10 };
        var existing = new List<IngredientEntity>
        {
            new() { Id = 8, Name = "SALT", Quantity = 5, FoodEntityId = 10 }
        };

        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(existing.AsQueryable());

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_MapsCorrectly()
    {
        var createDto = new IngredientCreateDto { Name = "Oil", Quantity = 1, FoodEntityId = 11 };
        var entity = new IngredientEntity { Id = 20, Name = "Oil", Quantity = 1, FoodEntityId = 11 };
        var dto = new IngredientDto { Id = 20, Name = "Oil", Quantity = 1, FoodEntityId = 11 };

        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(new List<IngredientEntity>().AsQueryable());
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _repoMock.Setup(r => r.GetSingle(20)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.CreateAsync(createDto);

        Assert.Equal(20, result.Id);
        Assert.Equal(11, result.FoodEntityId);
        _mapperMock.Verify(m => m.Map<IngredientEntity>(createDto), Times.Once);
        _mapperMock.Verify(m => m.Map<IngredientDto>(entity), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Success()
    {
        var updateDto = new IngredientUpdateDto { Name = "Brown Sugar", Quantity = 3, FoodEntityId = 7 };
        var existingEntity = new IngredientEntity { Id = 9, Name = "Sugar", Quantity = 1, FoodEntityId = 7 };
        var updatedEntity = new IngredientEntity { Id = 9, Name = "Brown Sugar", Quantity = 3, FoodEntityId = 7 };
        var dto = new IngredientDto { Id = 9, Name = "Brown Sugar", Quantity = 3, FoodEntityId = 7 };

        _repoMock.Setup(r => r.GetSingle(9)).Returns(existingEntity);
        _repoMock.Setup(r => r.Update(9, It.IsAny<IngredientEntity>())).Returns(updatedEntity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(updatedEntity)).Returns(dto);

        var result = await _service.UpdateAsync(9, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Brown Sugar", result.Name);
        Assert.Equal(3, result.Quantity);
    }

    [Fact]
    public async Task UpdateAsync_NotFound()
    {
        _repoMock.Setup(r => r.GetSingle(404)).Returns((IngredientEntity)null);

        var result = await _service.UpdateAsync(404, new IngredientUpdateDto { Name = "Any", Quantity = 1, FoodEntityId = 1 });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate()
    {
        var updateDto = new IngredientUpdateDto { Name = null, Quantity = 7, FoodEntityId = 0 };
        var existingEntity = new IngredientEntity { Id = 10, Name = "Salt", Quantity = 1, FoodEntityId = 2 };
        var updatedEntity = new IngredientEntity { Id = 10, Name = "Salt", Quantity = 7, FoodEntityId = 2 };
        var dto = new IngredientDto { Id = 10, Name = "Salt", Quantity = 7, FoodEntityId = 2 };

        _repoMock.Setup(r => r.GetSingle(10)).Returns(existingEntity);
        _repoMock.Setup(r => r.Update(10, It.IsAny<IngredientEntity>()))
            .Returns<int, IngredientEntity>((_, entity) => entity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(It.IsAny<IngredientEntity>())).Returns(dto);

        var result = await _service.UpdateAsync(10, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Salt", result.Name);
        Assert.Equal(7, result.Quantity);
    }

    [Fact]
    public async Task DeleteAsync_Success()
    {
        var entity = new IngredientEntity { Id = 12, Name = "Milk", Quantity = 1, FoodEntityId = 3 };

        _repoMock.Setup(r => r.GetSingle(12)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.DeleteAsync(12);

        Assert.True(result);
        _repoMock.Verify(r => r.Delete(12), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NotFound()
    {
        _repoMock.Setup(r => r.GetSingle(999)).Returns((IngredientEntity)null);

        var result = await _service.DeleteAsync(999);

        Assert.False(result);
        _repoMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task SearchAsync_FindsMatches()
    {
        var entities = new List<IngredientEntity>
        {
            new() { Id = 1, Name = "Sea Salt", Quantity = 1, FoodEntityId = 1 }
        };
        var dtos = new List<IngredientDto>
        {
            new() { Id = 1, Name = "Sea Salt", Quantity = 1, FoodEntityId = 1 }
        };

        _repoMock.Setup(r => r.GetAll(It.Is<QueryParameters>(q => q.Query == "salt")))
            .Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.SearchAsync("salt");

        Assert.Single(result);
        Assert.Contains("Salt", result.First().Name);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty()
    {
        _repoMock.Setup(r => r.GetAll(It.Is<QueryParameters>(q => q.Query == "nomatch")))
            .Returns(new List<IngredientEntity>().AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns(new List<IngredientDto>());

        var result = await _service.SearchAsync("nomatch");

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_CaseInsensitive()
    {
        var entities = new List<IngredientEntity>
        {
            new() { Id = 2, Name = "Pepper", Quantity = 1, FoodEntityId = 1 }
        };
        var dtos = new List<IngredientDto>
        {
            new() { Id = 2, Name = "Pepper", Quantity = 1, FoodEntityId = 1 }
        };

        _repoMock.Setup(r => r.GetAll(It.Is<QueryParameters>(q => q.Query == "PEPPER")))
            .Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.SearchAsync("PEPPER");

        Assert.Single(result);
        Assert.Equal("Pepper", result.First().Name);
    }
}
