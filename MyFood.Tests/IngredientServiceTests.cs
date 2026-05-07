using AutoMapper;
using Moq;
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
        var mapper = new MapperConfiguration(config => config.AddProfile<FoodMappings>()).CreateMapper();
        _service = new IngredientService(_repoMock.Object, mapper);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllIngredients()
    {
        var ingredients = new List<IngredientEntity>
        {
            CreateEntity(1, "Olive oil"),
            CreateEntity(2, "Flour")
        };
        _repoMock.Setup(repository => repository.GetAll()).Returns(ingredients);

        var result = (await _service.GetAllAsync(new QueryParameters())).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, ingredient => ingredient.Name == "Olive oil");
        Assert.Contains(result, ingredient => ingredient.Name == "Flour");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenRepositoryIsEmpty()
    {
        _repoMock.Setup(repository => repository.GetAll()).Returns(new List<IngredientEntity>());

        var result = await _service.GetAllAsync(new QueryParameters());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_AppliesPagination()
    {
        var ingredients = Enumerable.Range(1, 5)
            .Select(index => CreateEntity(index, $"Ingredient {index}"))
            .ToList();
        _repoMock.Setup(repository => repository.GetAll()).Returns(ingredients);

        var result = (await _service.GetAllAsync(new QueryParameters { Page = 2, PageCount = 2 })).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(3, result[0].Id);
        Assert.Equal(4, result[1].Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsIngredient_WhenFound()
    {
        _repoMock.Setup(repository => repository.GetById(1)).Returns(CreateEntity(1, "Rice"));

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Rice", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(repository => repository.GetById(99)).Returns((IngredientEntity?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_ForNegativeId()
    {
        var result = await _service.GetByIdAsync(-1);

        Assert.Null(result);
        _repoMock.Verify(repository => repository.GetById(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_CreatesIngredient_WhenValid()
    {
        _repoMock.Setup(repository => repository.GetAll()).Returns(new List<IngredientEntity>());
        _repoMock.Setup(repository => repository.Add(It.IsAny<IngredientEntity>()))
            .Callback<IngredientEntity>(ingredient => ingredient.Id = 10)
            .Returns((IngredientEntity ingredient) => ingredient);
        _repoMock.Setup(repository => repository.Save()).Returns(true);

        var result = await _service.CreateAsync(CreateDto("Milk"));

        Assert.Equal(10, result.Id);
        Assert.Equal("Milk", result.Name);
        _repoMock.Verify(repository => repository.Add(It.IsAny<IngredientEntity>()), Times.Once);
        _repoMock.Verify(repository => repository.Save(), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAsync_Throws_WhenNameIsMissing(string? name)
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(CreateDto(name)));
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenDuplicateNameExists()
    {
        _repoMock.Setup(repository => repository.GetAll()).Returns(new List<IngredientEntity>
        {
            CreateEntity(1, "Olive Oil")
        });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(CreateDto("olive oil")));
    }

    [Fact]
    public async Task CreateAsync_MapsFieldsCorrectly()
    {
        IngredientEntity? savedEntity = null;
        _repoMock.Setup(repository => repository.GetAll()).Returns(new List<IngredientEntity>());
        _repoMock.Setup(repository => repository.Add(It.IsAny<IngredientEntity>()))
            .Callback<IngredientEntity>(ingredient => savedEntity = ingredient)
            .Returns((IngredientEntity ingredient) => ingredient);
        _repoMock.Setup(repository => repository.Save()).Returns(true);

        var result = await _service.CreateAsync(new IngredientCreateDto
        {
            Name = "  Chicken  ",
            Quantity = 2,
            Unit = " g ",
            CaloriesPerUnit = 1.65m,
            Protein = 0.31m,
            Carbs = 0.01m,
            Fat = 0.04m
        });

        Assert.NotNull(savedEntity);
        Assert.Equal("Chicken", savedEntity.Name);
        Assert.Equal("g", savedEntity.Unit);
        Assert.Equal(2, result.Quantity);
        Assert.Equal(1.65m, result.CaloriesPerUnit);
        Assert.Equal(0.31m, result.Protein);
        Assert.Equal(0.01m, result.Carbs);
        Assert.Equal(0.04m, result.Fat);
        Assert.True(savedEntity.Created > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesIngredient_WhenFound()
    {
        var existing = CreateEntity(1, "Sugar");
        _repoMock.Setup(repository => repository.GetById(1)).Returns(existing);
        _repoMock.Setup(repository => repository.GetAll()).Returns(new List<IngredientEntity> { existing });
        _repoMock.Setup(repository => repository.Update(1, existing)).Returns(existing);
        _repoMock.Setup(repository => repository.Save()).Returns(true);

        var result = await _service.UpdateAsync(1, new IngredientUpdateDto
        {
            Name = "Brown sugar",
            Quantity = 2,
            Unit = "tbsp",
            CaloriesPerUnit = 52m,
            Protein = 0.1m,
            Carbs = 13m,
            Fat = 0.1m
        });

        Assert.NotNull(result);
        Assert.Equal("Brown sugar", result.Name);
        Assert.Equal("tbsp", result.Unit);
        Assert.Equal(52m, result.CaloriesPerUnit);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(repository => repository.GetById(99)).Returns((IngredientEntity?)null);

        var result = await _service.UpdateAsync(99, new IngredientUpdateDto { Name = "Missing" });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_AppliesPartialUpdate()
    {
        var existing = CreateEntity(1, "Butter", unit: "g", caloriesPerUnit: 7.17m);
        _repoMock.Setup(repository => repository.GetById(1)).Returns(existing);
        _repoMock.Setup(repository => repository.Update(1, existing)).Returns(existing);
        _repoMock.Setup(repository => repository.Save()).Returns(true);

        var result = await _service.UpdateAsync(1, new IngredientUpdateDto { Unit = "tbsp" });

        Assert.NotNull(result);
        Assert.Equal("Butter", result.Name);
        Assert.Equal("tbsp", result.Unit);
        Assert.Equal(7.17m, result.CaloriesPerUnit);
    }

    [Fact]
    public async Task DeleteAsync_DeletesIngredient_WhenFound()
    {
        _repoMock.Setup(repository => repository.GetById(1)).Returns(CreateEntity(1, "Salt"));
        _repoMock.Setup(repository => repository.Save()).Returns(true);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
        _repoMock.Verify(repository => repository.Delete(1), Times.Once);
        _repoMock.Verify(repository => repository.Save(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        _repoMock.Setup(repository => repository.GetById(99)).Returns((IngredientEntity?)null);

        var result = await _service.DeleteAsync(99);

        Assert.False(result);
        _repoMock.Verify(repository => repository.Delete(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task SearchAsync_FindsMatches()
    {
        _repoMock.Setup(repository => repository.GetAll()).Returns(new List<IngredientEntity>
        {
            CreateEntity(1, "Olive oil"),
            CreateEntity(2, "Coconut milk"),
            CreateEntity(3, "Flour")
        });

        var result = (await _service.SearchAsync("oil")).ToList();

        Assert.Single(result);
        Assert.Equal("Olive oil", result[0].Name);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
    {
        _repoMock.Setup(repository => repository.GetAll()).Returns(new List<IngredientEntity>
        {
            CreateEntity(1, "Olive oil")
        });

        var result = await _service.SearchAsync("banana");

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_IsCaseInsensitive()
    {
        _repoMock.Setup(repository => repository.GetAll()).Returns(new List<IngredientEntity>
        {
            CreateEntity(1, "Greek Yogurt")
        });

        var result = (await _service.SearchAsync("yOgUrT")).ToList();

        Assert.Single(result);
        Assert.Equal("Greek Yogurt", result[0].Name);
    }

    private static IngredientCreateDto CreateDto(string? name)
    {
        return new IngredientCreateDto
        {
            Name = name!,
            Quantity = 1,
            Unit = "g",
            CaloriesPerUnit = 1m,
            Protein = 1m,
            Carbs = 1m,
            Fat = 1m
        };
    }

    private static IngredientEntity CreateEntity(
        int id,
        string name,
        string unit = "g",
        decimal caloriesPerUnit = 1m)
    {
        return new IngredientEntity
        {
            Id = id,
            Name = name,
            Quantity = 1,
            Unit = unit,
            CaloriesPerUnit = caloriesPerUnit,
            Protein = 1m,
            Carbs = 1m,
            Fat = 1m,
            Created = DateTime.UtcNow
        };
    }
}
