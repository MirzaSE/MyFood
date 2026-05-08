using AutoMapper;
using Moq;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

public class IngredientServiceTests
{
    private readonly Mock<IIngredientRepository> _ingredientRepoMock = new();
    private readonly Mock<IFoodRepository> _foodRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly IngredientService _service;

    public IngredientServiceTests()
    {
        _service = new IngredientService(_ingredientRepoMock.Object, _foodRepoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllIngredients()
    {
        var entities = new List<IngredientEntity>
        {
            new() { Id = 1, Name = "Salt", Quantity = 2, FoodEntityId = 1 },
            new() { Id = 2, Name = "Pepper", Quantity = 1, FoodEntityId = 1 }
        };
        var dtos = entities.Select(x => new IngredientDto { Id = x.Id, Name = x.Name, Quantity = x.Quantity, FoodEntityId = x.FoodEntityId });

        _ingredientRepoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = (await _service.GetAllAsync(new QueryParameters())).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoIngredientsExist()
    {
        _ingredientRepoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(Enumerable.Empty<IngredientEntity>().AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(Enumerable.Empty<IngredientDto>());

        var result = await _service.GetAllAsync(new QueryParameters());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_UsesPaginationParameters()
    {
        QueryParameters? captured = null;
        _ingredientRepoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>()))
            .Callback<QueryParameters>(qp => captured = qp)
            .Returns(Enumerable.Empty<IngredientEntity>().AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(Enumerable.Empty<IngredientDto>());

        await _service.GetAllAsync(new QueryParameters { Page = 2, PageCount = 5 });

        Assert.NotNull(captured);
        Assert.Equal(2, captured!.Page);
        Assert.Equal(5, captured.PageCount);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsIngredient_WhenFound()
    {
        var entity = new IngredientEntity { Id = 10, Name = "Sugar", Quantity = 3, FoodEntityId = 1 };
        var dto = new IngredientDto { Id = 10, Name = "Sugar", Quantity = 3, FoodEntityId = 1 };

        _ingredientRepoMock.Setup(r => r.GetSingle(10)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.GetByIdAsync(10);

        Assert.NotNull(result);
        Assert.Equal("Sugar", result!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _ingredientRepoMock.Setup(r => r.GetSingle(404)).Returns((IngredientEntity?)null);

        var result = await _service.GetByIdAsync(404);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_ForNegativeId()
    {
        var result = await _service.GetByIdAsync(-1);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesIngredientSuccessfully()
    {
        var createDto = new IngredientCreateDto { Name = "Garlic", Quantity = 2, FoodEntityId = 1 };
        var entity = new IngredientEntity { Id = 44, Name = "Garlic", Quantity = 2, FoodEntityId = 1 };
        var dto = new IngredientDto { Id = 44, Name = "Garlic", Quantity = 2, FoodEntityId = 1 };

        _foodRepoMock.Setup(r => r.GetSingle(1)).Returns(new FoodEntity { Id = 1 });
        _ingredientRepoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(Enumerable.Empty<IngredientEntity>().AsQueryable());
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _ingredientRepoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.CreateAsync(createDto);

        Assert.Equal("Garlic", result.Name);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenNameIsNull()
    {
        var createDto = new IngredientCreateDto { Name = null, Quantity = 1, FoodEntityId = 1 };
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenDuplicateExists()
    {
        var createDto = new IngredientCreateDto { Name = "Salt", Quantity = 1, FoodEntityId = 1 };
        var existing = new IngredientEntity { Name = "salt", Quantity = 1, FoodEntityId = 1 };

        _foodRepoMock.Setup(r => r.GetSingle(1)).Returns(new FoodEntity { Id = 1 });
        _ingredientRepoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(new[] { existing }.AsQueryable());

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_MapsAndPersistsCorrectly()
    {
        var createDto = new IngredientCreateDto { Name = "Oil", Quantity = 1, FoodEntityId = 2 };
        var entity = new IngredientEntity { Name = "Oil", Quantity = 1, FoodEntityId = 2 };

        _foodRepoMock.Setup(r => r.GetSingle(2)).Returns(new FoodEntity { Id = 2 });
        _ingredientRepoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(Enumerable.Empty<IngredientEntity>().AsQueryable());
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _ingredientRepoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(new IngredientDto());

        await _service.CreateAsync(createDto);

        _mapperMock.Verify(m => m.Map<IngredientEntity>(createDto), Times.Once);
        _ingredientRepoMock.Verify(r => r.Add(entity), Times.Once);
        _ingredientRepoMock.Verify(r => r.Save(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesIngredientSuccessfully()
    {
        var existing = new IngredientEntity { Id = 5, Name = "Tomato", Quantity = 1, FoodEntityId = 1 };
        var update = new IngredientUpdateDto { Name = "Tomato Paste", Quantity = 2, FoodEntityId = 1 };

        _ingredientRepoMock.Setup(r => r.GetSingle(5)).Returns(existing);
        _ingredientRepoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(It.IsAny<IngredientEntity>()))
            .Returns<IngredientEntity>(x => new IngredientDto { Id = x.Id, Name = x.Name, Quantity = x.Quantity, FoodEntityId = x.FoodEntityId });

        var result = await _service.UpdateAsync(5, update);

        Assert.NotNull(result);
        Assert.Equal("Tomato Paste", result!.Name);
        Assert.Equal(2, result.Quantity);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenIngredientMissing()
    {
        _ingredientRepoMock.Setup(r => r.GetSingle(88)).Returns((IngredientEntity?)null);
        var result = await _service.UpdateAsync(88, new IngredientUpdateDto());
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate_PreservesNameWhenNull()
    {
        var existing = new IngredientEntity { Id = 9, Name = "Flour", Quantity = 2, FoodEntityId = 1 };
        var update = new IngredientUpdateDto { Name = null, Quantity = 4, FoodEntityId = 1 };

        _ingredientRepoMock.Setup(r => r.GetSingle(9)).Returns(existing);
        _ingredientRepoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(It.IsAny<IngredientEntity>()))
            .Returns<IngredientEntity>(x => new IngredientDto { Id = x.Id, Name = x.Name, Quantity = x.Quantity, FoodEntityId = x.FoodEntityId });

        var result = await _service.UpdateAsync(9, update);

        Assert.NotNull(result);
        Assert.Equal("Flour", result!.Name);
        Assert.Equal(4, result.Quantity);
    }

    [Fact]
    public async Task DeleteAsync_DeletesIngredient_WhenFound()
    {
        _ingredientRepoMock.Setup(r => r.GetSingle(6)).Returns(new IngredientEntity { Id = 6 });
        _ingredientRepoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.DeleteAsync(6);

        Assert.True(result);
        _ingredientRepoMock.Verify(r => r.Delete(6), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenMissing()
    {
        _ingredientRepoMock.Setup(r => r.GetSingle(101)).Returns((IngredientEntity?)null);
        var result = await _service.DeleteAsync(101);
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatches()
    {
        var entities = new List<IngredientEntity> { new() { Id = 1, Name = "Sugar", Quantity = 1, FoodEntityId = 1 } };
        var dtos = new List<IngredientDto> { new() { Id = 1, Name = "Sugar", Quantity = 1, FoodEntityId = 1 } };

        _ingredientRepoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

        var result = await _service.SearchAsync("sug", new QueryParameters());

        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
    {
        _ingredientRepoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(Enumerable.Empty<IngredientEntity>().AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(Enumerable.Empty<IngredientDto>());

        var result = await _service.SearchAsync("none", new QueryParameters());

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_IsCaseInsensitive()
    {
        QueryParameters? captured = null;
        _ingredientRepoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>()))
            .Callback<QueryParameters>(qp => captured = qp)
            .Returns(Enumerable.Empty<IngredientEntity>().AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(Enumerable.Empty<IngredientDto>());

        await _service.SearchAsync("SALT", new QueryParameters());

        Assert.NotNull(captured);
        Assert.Equal("SALT", captured!.Query);
    }
}
