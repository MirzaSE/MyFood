using AutoMapper;
using Moq;
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

        _mapperMock
            .Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<object>()))
            .Returns((object source) =>
            {
                var ingredients = source as IEnumerable<IngredientEntity> ?? new List<IngredientEntity>();
                return ingredients.Select(ToDto).ToList();
            });

        _mapperMock
            .Setup(m => m.Map<IngredientDto>(It.IsAny<IngredientEntity>()))
            .Returns((IngredientEntity entity) => ToDto(entity));

        _mapperMock
            .Setup(m => m.Map<IngredientEntity>(It.IsAny<CreateIngredientDto>()))
            .Returns((CreateIngredientDto dto) => new IngredientEntity
            {
                Name = dto.Name,
                Unit = dto.Unit,
                CaloriesPerUnit = dto.CaloriesPerUnit,
                Protein = dto.Protein,
                Carbs = dto.Carbs,
                Fat = dto.Fat,
                Quantity = dto.Quantity,
                FoodEntityId = dto.FoodEntityId
            });
    }

    private static IngredientDto ToDto(IngredientEntity entity)
    {
        return new IngredientDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Unit = entity.Unit,
            CaloriesPerUnit = entity.CaloriesPerUnit,
            Protein = entity.Protein,
            Carbs = entity.Carbs,
            Fat = entity.Fat,
            Quantity = entity.Quantity,
            FoodEntityId = entity.FoodEntityId
        };
    }

    private static List<IngredientEntity> SampleIngredients()
    {
        return new List<IngredientEntity>
        {
            new IngredientEntity
            {
                Id = 1,
                Name = "Apple",
                Unit = "g",
                CaloriesPerUnit = 0.52m,
                Protein = 0.3m,
                Carbs = 14m,
                Fat = 0.2m
            },
            new IngredientEntity
            {
                Id = 2,
                Name = "Chicken Breast",
                Unit = "g",
                CaloriesPerUnit = 1.65m,
                Protein = 31m,
                Carbs = 0m,
                Fat = 3.6m
            },
            new IngredientEntity
            {
                Id = 3,
                Name = "Rice",
                Unit = "g",
                CaloriesPerUnit = 1.3m,
                Protein = 2.7m,
                Carbs = 28m,
                Fat = 0.3m
            }
        };
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllIngredients()
    {
        var ingredients = SampleIngredients();

        _repoMock.Setup(r => r.GetAll()).Returns(ingredients.AsQueryable());

        var result = await _service.GetAllAsync(1, 10);

        Assert.Equal(3, result.Count());
        Assert.Contains(result, i => i.Name == "Apple");
        Assert.Contains(result, i => i.Name == "Chicken Breast");
        Assert.Contains(result, i => i.Name == "Rice");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoIngredientsExist()
    {
        _repoMock.Setup(r => r.GetAll()).Returns(new List<IngredientEntity>().AsQueryable());

        var result = await _service.GetAllAsync(1, 10);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_PaginationWorks()
    {
        var ingredients = SampleIngredients();

        _repoMock.Setup(r => r.GetAll()).Returns(ingredients.AsQueryable());

        var result = await _service.GetAllAsync(2, 1);

        Assert.Single(result);
        Assert.Equal("Chicken Breast", result.First().Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsIngredient_WhenFound()
    {
        var ingredient = SampleIngredients().First();

        _repoMock.Setup(r => r.GetById(1)).Returns(ingredient);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("Apple", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetById(99)).Returns((IngredientEntity?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_ForNegativeId()
    {
        var result = await _service.GetByIdAsync(-1);

        Assert.Null(result);
        _repoMock.Verify(r => r.GetById(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_CreatesIngredientSuccessfully()
    {
        var dto = new CreateIngredientDto
        {
            Name = " Milk ",
            Unit = " ml ",
            CaloriesPerUnit = 0.42m,
            Protein = 3.4m,
            Carbs = 5m,
            Fat = 1m
        };

        _repoMock.Setup(r => r.GetByName("Milk")).Returns((IngredientEntity?)null);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Milk", result.Name);
        Assert.Equal("ml", result.Unit);

        _repoMock.Verify(r => r.Add(It.Is<IngredientEntity>(i =>
            i.Name == "Milk" &&
            i.Unit == "ml" &&
            i.CaloriesPerUnit == 0.42m
        )), Times.Once);

        _repoMock.Verify(r => r.Save(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ThrowsArgumentException_WhenNameIsNullOrEmpty()
    {
        var dto = new CreateIngredientDto
        {
            Name = "",
            Unit = "g",
            CaloriesPerUnit = 1m
        };

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));

        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_ThrowsInvalidOperationException_WhenDuplicateExists()
    {
        var dto = new CreateIngredientDto
        {
            Name = "Apple",
            Unit = "g",
            CaloriesPerUnit = 0.52m
        };

        _repoMock.Setup(r => r.GetByName("Apple")).Returns(new IngredientEntity
        {
            Id = 1,
            Name = "Apple",
            Unit = "g",
            CaloriesPerUnit = 0.52m
        });

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));

        Assert.Contains("already exists", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_MapsDtoToEntityCorrectly()
    {
        var dto = new CreateIngredientDto
        {
            Name = "Banana",
            Unit = "g",
            CaloriesPerUnit = 0.89m,
            Protein = 1.1m,
            Carbs = 23m,
            Fat = 0.3m,
            Quantity = 100m,
            FoodEntityId = 5
        };

        _repoMock.Setup(r => r.GetByName("Banana")).Returns((IngredientEntity?)null);
        _repoMock.Setup(r => r.Save()).Returns(true);

        await _service.CreateAsync(dto);

        _repoMock.Verify(r => r.Add(It.Is<IngredientEntity>(i =>
            i.Name == "Banana" &&
            i.Unit == "g" &&
            i.CaloriesPerUnit == 0.89m &&
            i.Protein == 1.1m &&
            i.Carbs == 23m &&
            i.Fat == 0.3m &&
            i.Quantity == 100m &&
            i.FoodEntityId == 5
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesIngredientSuccessfully()
    {
        var entity = new IngredientEntity
        {
            Id = 1,
            Name = "Old Apple",
            Unit = "g",
            CaloriesPerUnit = 0.5m,
            Protein = 0.2m,
            Carbs = 10m,
            Fat = 0.1m
        };

        var dto = new UpdateIngredientDto
        {
            Name = "Green Apple",
            Unit = "piece",
            CaloriesPerUnit = 80m,
            Protein = 0.4m,
            Carbs = 20m,
            Fat = 0.2m
        };

        _repoMock.Setup(r => r.GetById(1)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.UpdateAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("Green Apple", result!.Name);
        Assert.Equal("piece", result.Unit);
        Assert.Equal(80m, result.CaloriesPerUnit);

        _repoMock.Verify(r => r.Update(entity), Times.Once);
        _repoMock.Verify(r => r.Save(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenIngredientNotFound()
    {
        _repoMock.Setup(r => r.GetById(99)).Returns((IngredientEntity?)null);

        var result = await _service.UpdateAsync(99, new UpdateIngredientDto
        {
            Name = "Missing"
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_AllowsPartialUpdate()
    {
        var entity = new IngredientEntity
        {
            Id = 1,
            Name = "Apple",
            Unit = "g",
            CaloriesPerUnit = 0.52m,
            Protein = 0.3m,
            Carbs = 14m,
            Fat = 0.2m
        };

        var dto = new UpdateIngredientDto
        {
            Name = "Red Apple"
        };

        _repoMock.Setup(r => r.GetById(1)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.UpdateAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("Red Apple", result!.Name);
        Assert.Equal("g", result.Unit);
        Assert.Equal(0.52m, result.CaloriesPerUnit);
    }

    [Fact]
    public async Task DeleteAsync_DeletesIngredientSuccessfully()
    {
        var entity = new IngredientEntity
        {
            Id = 1,
            Name = "Apple",
            Unit = "g",
            CaloriesPerUnit = 0.52m
        };

        _repoMock.Setup(r => r.GetById(1)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
        _repoMock.Verify(r => r.Delete(entity), Times.Once);
        _repoMock.Verify(r => r.Save(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenIngredientNotFound()
    {
        _repoMock.Setup(r => r.GetById(99)).Returns((IngredientEntity?)null);

        var result = await _service.DeleteAsync(99);

        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatches()
    {
        var ingredients = SampleIngredients();

        _repoMock.Setup(r => r.GetAll()).Returns(ingredients.AsQueryable());

        var result = await _service.SearchAsync("Rice");

        Assert.Single(result);
        Assert.Equal("Rice", result.First().Name);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
    {
        var ingredients = SampleIngredients();

        _repoMock.Setup(r => r.GetAll()).Returns(ingredients.AsQueryable());

        var result = await _service.SearchAsync("Chocolate");

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_IsCaseInsensitive()
    {
        var ingredients = SampleIngredients();

        _repoMock.Setup(r => r.GetAll()).Returns(ingredients.AsQueryable());

        var result = await _service.SearchAsync("cHiCkEn");

        Assert.Single(result);
        Assert.Equal("Chicken Breast", result.First().Name);
    }
}