using Moq;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

public class IngredientServiceTests
{
    private readonly Mock<IIngredientRepository> _repoMock = new();
    private readonly IngredientService _service;

    public IngredientServiceTests()
    {
        _service = new IngredientService(_repoMock.Object);
    }

    private static List<IngredientEntity> SampleIngredients() =>
        new()
        {
            new IngredientEntity { Id = 1, Name = "Apple", Unit = "g", CaloriesPerUnit = 0.52, Protein = 0.3, Carbs = 14, Fat = 0.2 },
            new IngredientEntity { Id = 2, Name = "Banana", Unit = "g", CaloriesPerUnit = 0.89, Protein = 1.1, Carbs = 23, Fat = 0.3 },
            new IngredientEntity { Id = 3, Name = "Chicken", Unit = "g", CaloriesPerUnit = 1.65, Protein = 31, Carbs = 0, Fat = 3.6 }
        };

    [Fact]
    public async Task GetAllAsync_ReturnsAll()
    {
        _repoMock.Setup(r => r.GetAll()).Returns(SampleIngredients().AsQueryable());

        var result = await _service.GetAllAsync(1, 10);

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty()
    {
        _repoMock.Setup(r => r.GetAll()).Returns(new List<IngredientEntity>().AsQueryable());

        var result = await _service.GetAllAsync(1, 10);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_PaginationWorks()
    {
        _repoMock.Setup(r => r.GetAll()).Returns(SampleIngredients().AsQueryable());

        var result = await _service.GetAllAsync(2, 1);

        Assert.Single(result);
        Assert.Equal("Banana", result.First().Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsIngredient_WhenFound()
    {
        var entity = SampleIngredients().First();
        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Apple", result!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

        var result = await _service.GetByIdAsync(99);

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
        var dto = new IngredientCreateDto
        {
            Name = "Rice",
            Unit = "g",
            CaloriesPerUnit = 1.3,
            Protein = 2.7,
            Carbs = 28,
            Fat = 0.3
        };

        _repoMock.Setup(r => r.GetAll()).Returns(new List<IngredientEntity>().AsQueryable());
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.CreateAsync(dto);

        Assert.Equal("Rice", result.Name);
        _repoMock.Verify(r => r.Add(It.IsAny<IngredientEntity>()), Times.Once);
        _repoMock.Verify(r => r.Save(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ThrowsError_WhenNameIsNull()
    {
        var dto = new IngredientCreateDto { Name = "", Unit = "g" };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsError_WhenDuplicateExists()
    {
        var dto = new IngredientCreateDto { Name = "Apple", Unit = "g" };

        _repoMock.Setup(r => r.GetAll()).Returns(SampleIngredients().AsQueryable());

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_MapsCorrectly()
    {
        IngredientEntity? captured = null;

        var dto = new IngredientCreateDto
        {
            Name = "Milk",
            Unit = "ml",
            CaloriesPerUnit = 0.42,
            Protein = 3.4,
            Carbs = 5,
            Fat = 1
        };

        _repoMock.Setup(r => r.GetAll()).Returns(new List<IngredientEntity>().AsQueryable());
        _repoMock.Setup(r => r.Add(It.IsAny<IngredientEntity>()))
            .Callback<IngredientEntity>(i => captured = i);
        _repoMock.Setup(r => r.Save()).Returns(true);

        await _service.CreateAsync(dto);

        Assert.NotNull(captured);
        Assert.Equal("Milk", captured!.Name);
        Assert.Equal("ml", captured.Unit);
        Assert.Equal(0.42, captured.CaloriesPerUnit);
        Assert.Equal(3.4, captured.Protein);
        Assert.Equal(5, captured.Carbs);
        Assert.Equal(1, captured.Fat);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesSuccessfully()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Old", Unit = "g" };
        var dto = new IngredientUpdateDto { Name = "Updated", Unit = "kg" };

        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _repoMock.Setup(r => r.Update(1, entity)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.UpdateAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("Updated", result!.Name);
        Assert.Equal("kg", result.Unit);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

        var result = await _service.UpdateAsync(99, new IngredientUpdateDto { Name = "New" });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdateWorks()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Apple", Unit = "g", CaloriesPerUnit = 0.52 };
        var dto = new IngredientUpdateDto { CaloriesPerUnit = 1.0 };

        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _repoMock.Setup(r => r.Update(1, entity)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.UpdateAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("Apple", result!.Name);
        Assert.Equal("g", result.Unit);
        Assert.Equal(1.0, result.CaloriesPerUnit);
    }

    [Fact]
    public async Task DeleteAsync_DeletesSuccessfully()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Apple", Unit = "g" };

        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
        _repoMock.Verify(r => r.Delete(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

        var result = await _service.DeleteAsync(99);

        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_FindsMatches()
    {
        _repoMock.Setup(r => r.GetAll()).Returns(SampleIngredients().AsQueryable());

        var result = await _service.SearchAsync("App");

        Assert.Single(result);
        Assert.Equal("Apple", result.First().Name);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
    {
        _repoMock.Setup(r => r.GetAll()).Returns(SampleIngredients().AsQueryable());

        var result = await _service.SearchAsync("zzz");

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_IsCaseInsensitive()
    {
        _repoMock.Setup(r => r.GetAll()).Returns(SampleIngredients().AsQueryable());

        var result = await _service.SearchAsync("apple");

        Assert.Single(result);
        Assert.Equal("Apple", result.First().Name);
    }
}