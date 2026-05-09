using AutoMapper;
using Moq;
using MyFood.Api.MappingProfiles;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

public class IngredientServiceTests
{
    private readonly Mock<IIngredientRepository> _repoMock = new();
    private readonly IngredientService _service;

    public IngredientServiceTests()
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<FoodMappings>()).CreateMapper();
        _service = new IngredientService(_repoMock.Object, mapper);
    }

    [Fact]
    public async Task GetAllIngredientsAsync_ReturnsAllIngredients()
    {
        var ingredients = new List<IngredientEntity>
        {
            new() { Id = 1, Name = "Flour", Quantity = 2 },
            new() { Id = 2, Name = "Eggs", Quantity = 6 }
        };
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(ingredients.AsQueryable());

        var result = (await _service.GetAllIngredientsAsync(new QueryParameters())).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Name == "Flour");
    }

    [Fact]
    public async Task GetAllIngredientsAsync_ReturnsEmpty_WhenRepositoryIsEmpty()
    {
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(Array.Empty<IngredientEntity>().AsQueryable());

        var result = await _service.GetAllIngredientsAsync(new QueryParameters());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllIngredientsAsync_PassesPaginationParametersToRepository()
    {
        QueryParameters? capturedParameters = null;
        var parameters = new QueryParameters { Page = 2, PageCount = 5 };
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>()))
            .Callback<QueryParameters>(x => capturedParameters = x)
            .Returns(Array.Empty<IngredientEntity>().AsQueryable());

        await _service.GetAllIngredientsAsync(parameters);

        Assert.Same(parameters, capturedParameters);
    }

    [Fact]
    public async Task GetIngredientByIdAsync_ReturnsMappedDto_WhenFound()
    {
        _repoMock.Setup(r => r.GetSingle(7)).Returns(new IngredientEntity { Id = 7, Name = "Milk", Quantity = 1 });

        var result = await _service.GetIngredientByIdAsync(7);

        Assert.NotNull(result);
        Assert.Equal(7, result.Id);
        Assert.Equal("Milk", result.Name);
    }

    [Fact]
    public async Task GetIngredientByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null!);

        var result = await _service.GetIngredientByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetIngredientByIdAsync_Throws_WhenIdIsNegative()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.GetIngredientByIdAsync(-1));
    }

    [Fact]
    public async Task CreateIngredientAsync_AddsAndReturnsCreatedDto()
    {
        var createDto = new IngredientCreateDto { Name = "Sugar", Quantity = 3 };
        _repoMock.Setup(r => r.SearchIngredientsByName("Sugar")).Returns(Array.Empty<IngredientEntity>());
        _repoMock.Setup(r => r.Add(It.IsAny<IngredientEntity>()))
            .Callback<IngredientEntity>(x => x.Id = 12);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _repoMock.Setup(r => r.GetSingle(12)).Returns(new IngredientEntity { Id = 12, Name = "Sugar", Quantity = 3 });

        var result = await _service.CreateIngredientAsync(createDto);

        Assert.Equal(12, result.Id);
        Assert.Equal("Sugar", result.Name);
        Assert.Equal(3, result.Quantity);
        _repoMock.Verify(r => r.Add(It.Is<IngredientEntity>(x => x.Name == "Sugar" && x.Quantity == 3)), Times.Once);
    }

    [Fact]
    public async Task CreateIngredientAsync_Throws_WhenNameIsNull()
    {
        var createDto = new IngredientCreateDto { Name = null, Quantity = 1 };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateIngredientAsync(createDto));
    }

    [Fact]
    public async Task CreateIngredientAsync_Throws_WhenDuplicateNameExists()
    {
        var createDto = new IngredientCreateDto { Name = "Salt", Quantity = 1 };
        _repoMock.Setup(r => r.SearchIngredientsByName("Salt"))
            .Returns(new[] { new IngredientEntity { Id = 1, Name = "salt", Quantity = 2 } });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateIngredientAsync(createDto));
    }

    [Fact]
    public async Task CreateIngredientAsync_MapsDtoFieldsCorrectly()
    {
        var createDto = new IngredientCreateDto { Name = "Butter", Quantity = 4 };
        IngredientEntity? added = null;
        _repoMock.Setup(r => r.SearchIngredientsByName("Butter")).Returns(Array.Empty<IngredientEntity>());
        _repoMock.Setup(r => r.Add(It.IsAny<IngredientEntity>()))
            .Callback<IngredientEntity>(x =>
            {
                x.Id = 20;
                added = x;
            });
        _repoMock.Setup(r => r.Save()).Returns(true);
        _repoMock.Setup(r => r.GetSingle(20)).Returns(() => added!);

        await _service.CreateIngredientAsync(createDto);

        Assert.NotNull(added);
        Assert.Equal("Butter", added.Name);
        Assert.Equal(4, added.Quantity);
    }

    [Fact]
    public async Task UpdateIngredientAsync_UpdatesAndReturnsDto()
    {
        var existing = new IngredientEntity { Id = 5, Name = "Old", Quantity = 1 };
        var updateDto = new IngredientUpdateDto { Name = "New", Quantity = 2 };
        _repoMock.Setup(r => r.GetSingle(5)).Returns(existing);
        _repoMock.Setup(r => r.Update(5, existing)).Returns(existing);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.UpdateIngredientAsync(5, updateDto);

        Assert.NotNull(result);
        Assert.Equal("New", result.Name);
        Assert.Equal(2, result.Quantity);
    }

    [Fact]
    public async Task UpdateIngredientAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(404)).Returns((IngredientEntity?)null!);

        var result = await _service.UpdateIngredientAsync(404, new IngredientUpdateDto { Name = "Missing", Quantity = 1 });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateIngredientAsync_AllowsPartialNameUpdate()
    {
        var existing = new IngredientEntity { Id = 8, Name = "Keep", Quantity = 1 };
        var updateDto = new IngredientUpdateDto { Name = null, Quantity = 10 };
        _repoMock.Setup(r => r.GetSingle(8)).Returns(existing);
        _repoMock.Setup(r => r.Update(8, existing)).Returns(existing);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.UpdateIngredientAsync(8, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Keep", result.Name);
        Assert.Equal(10, result.Quantity);
    }

    [Fact]
    public async Task DeleteIngredientAsync_DeletesAndReturnsTrue()
    {
        _repoMock.Setup(r => r.GetSingle(3)).Returns(new IngredientEntity { Id = 3, Name = "Oil" });
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.DeleteIngredientAsync(3);

        Assert.True(result);
        _repoMock.Verify(r => r.Delete(3), Times.Once);
    }

    [Fact]
    public async Task DeleteIngredientAsync_ReturnsFalse_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(300)).Returns((IngredientEntity?)null!);

        var result = await _service.DeleteIngredientAsync(300);

        Assert.False(result);
        _repoMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task SearchIngredientsByNameAsync_ReturnsMatches()
    {
        _repoMock.Setup(r => r.SearchIngredientsByName("su"))
            .Returns(new[] { new IngredientEntity { Id = 1, Name = "Sugar", Quantity = 1 } });

        var result = (await _service.SearchIngredientsByNameAsync("su")).ToList();

        Assert.Single(result);
        Assert.Equal("Sugar", result[0].Name);
    }

    [Fact]
    public async Task SearchIngredientsByNameAsync_ReturnsEmpty_WhenNoMatches()
    {
        _repoMock.Setup(r => r.SearchIngredientsByName("missing")).Returns(Array.Empty<IngredientEntity>());

        var result = await _service.SearchIngredientsByNameAsync("missing");

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchIngredientsByNameAsync_AllowsCaseInsensitiveRepositoryMatches()
    {
        _repoMock.Setup(r => r.SearchIngredientsByName("SUGAR"))
            .Returns(new[] { new IngredientEntity { Id = 4, Name = "sugar", Quantity = 1 } });

        var result = (await _service.SearchIngredientsByNameAsync("SUGAR")).ToList();

        Assert.Single(result);
        Assert.Equal("sugar", result[0].Name);
    }
}
