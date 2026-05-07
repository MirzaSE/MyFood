using Moq;
using MyFood.Application.Interfaces;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

public class IngredientServiceTests
{
    private readonly Mock<IIngredientRepository> _repoMock;
    private readonly IngredientService _service;

    public IngredientServiceTests()
    {
        _repoMock = new Mock<IIngredientRepository>();
        _repoMock.Setup(r => r.ExistsByNameAsync(It.IsAny<string>())).ReturnsAsync(false);
        _repoMock.Setup(r => r.ExistsByNameExceptIdAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(false);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<IngredientEntity>())).Returns(Task.CompletedTask);
        _service = new IngredientService(_repoMock.Object);
    }

    private static IngredientEntity ValidNew(string name = "Salt") =>
        new()
        {
            Name = name,
            Unit = "g",
            CaloriesPerUnit = 1,
            Protein = 0.1m,
            Carbs = 0.1m,
            Fat = 0.1m,
        };

    [Fact]
    public async Task GetAllAsync_ReturnsAll()
    {
        _repoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<IngredientEntity>
            {
                new IngredientEntity { Id = 1, Name = "Salt" },
                new IngredientEntity { Id = 2, Name = "Sugar" }
            });

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty()
    {
        _repoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<IngredientEntity>());

        var result = await _service.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_PaginationWorks()
    {
        var data = Enumerable.Range(1, 10)
            .Select(i => new IngredientEntity { Id = i, Name = $"Item{i}" })
            .ToList();

        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(data);

        var result = (await _service.GetAllAsync(page: 2, pageSize: 3)).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal(new[] { 4, 5, 6 }, result.Select(x => x.Id));
        Assert.Equal(["Item4", "Item5", "Item6"], result.Select(x => x.Name));
    }

    [Fact]
    public async Task GetByIdAsync_Found()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new IngredientEntity { Id = 1, Name = "Salt" });

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Salt", result!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((IngredientEntity?)null);

        var result = await _service.GetByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_NegativeId()
    {
        var result = await _service.GetByIdAsync(-1);

        Assert.Null(result);
        _repoMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_Success()
    {
        var ingredient = ValidNew();

        _repoMock.Setup(r => r.CreateAsync(It.IsAny<IngredientEntity>()))
            .ReturnsAsync((IngredientEntity e) => new IngredientEntity
            {
                Id = 1,
                Name = e.Name,
                Unit = e.Unit,
                CaloriesPerUnit = e.CaloriesPerUnit,
                Protein = e.Protein,
                Carbs = e.Carbs,
                Fat = e.Fat,
                FoodEntityId = e.FoodEntityId,
            });

        var result = await _service.CreateAsync(ingredient);

        Assert.Equal("Salt", result.Name);
    }

    [Fact]
    public async Task CreateAsync_NullName_Error()
    {
        var ingredient = ValidNew();
        ingredient.Name = null!;

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateAsync(ingredient));
    }

    [Fact]
    public async Task CreateAsync_Duplicate_Error()
    {
        _repoMock.Setup(r => r.ExistsByNameAsync("Salt"))
            .ReturnsAsync(true);

        var ingredient = ValidNew();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.CreateAsync(ingredient));
    }

    [Fact]
    public async Task CreateAsync_MapsCorrectly()
    {
        var ingredient = ValidNew("  Salt  ");

        _repoMock.Setup(r => r.CreateAsync(It.Is<IngredientEntity>(e => e.Name == "Salt")))
            .ReturnsAsync((IngredientEntity e) => new IngredientEntity { Id = 99, Name = e.Name });

        var result = await _service.CreateAsync(ingredient);

        Assert.Equal("Salt", result.Name);
        Assert.Equal(99, result.Id);
        _repoMock.Verify(r => r.CreateAsync(It.Is<IngredientEntity>(e => e.Name == "Salt")), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_InvalidNutrition_Error()
    {
        var ingredient = ValidNew();
        ingredient.CaloriesPerUnit = 0;

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(ingredient));
    }

    [Fact]
    public async Task UpdateAsync_Success()
    {
        var existing = new IngredientEntity
        {
            Id = 1,
            Name = "Old",
            Unit = "g",
            CaloriesPerUnit = 1,
            Protein = 1,
            Carbs = 1,
            Fat = 1,
        };

        var incoming = new IngredientEntity
        {
            Name = "New",
            Unit = "g",
            CaloriesPerUnit = 2,
            Protein = 1,
            Carbs = 1,
            Fat = 1,
        };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        var result = await _service.UpdateAsync(1, incoming);

        Assert.NotNull(result);
        Assert.Equal("New", result!.Name);
        Assert.Equal(2, result.CaloriesPerUnit);
    }

    [Fact]
    public async Task UpdateAsync_NotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((IngredientEntity?)null);

        var result = await _service.UpdateAsync(1, ValidNew("New"));

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_DuplicateName_Error()
    {
        var existing = new IngredientEntity
        {
            Id = 1,
            Name = "Old",
            Unit = "g",
            CaloriesPerUnit = 1,
            Protein = 1,
            Carbs = 1,
            Fat = 1,
        };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.ExistsByNameExceptIdAsync("Taken", 1)).ReturnsAsync(true);

        var incoming = ValidNew("Taken");

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(1, incoming));
    }

    [Fact]
    public async Task DeleteAsync_Success()
    {
        _repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_NotFound()
    {
        _repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(false);

        var result = await _service.DeleteAsync(1);

        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_FindsMatches()
    {
        _repoMock.Setup(r => r.SearchAsync("sal"))
            .ReturnsAsync(new List<IngredientEntity>
            {
                new IngredientEntity { Name = "Salt" }
            });

        var result = await _service.SearchAsync("sal");

        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty()
    {
        _repoMock.Setup(r => r.SearchAsync("xyz"))
            .ReturnsAsync(new List<IngredientEntity>());

        var result = await _service.SearchAsync("xyz");

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_CaseInsensitive()
    {
        _repoMock.Setup(r => r.SearchAsync("salt"))
            .ReturnsAsync(new List<IngredientEntity>
            {
                new IngredientEntity { Name = "Salt" }
            });

        var result = await _service.SearchAsync("SALT");

        Assert.Single(result);
        _repoMock.Verify(r => r.SearchAsync("salt"), Times.Once);
    }
}
