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

        _mapperMock
            .Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns((IEnumerable<IngredientEntity> entities) => entities.Select(e => new IngredientDto
            {
                Id = e.Id,
                Name = e.Name,
                FoodEntityId = e.FoodEntityId
            }).ToList());

        _mapperMock
            .Setup(m => m.Map<IngredientDto>(It.IsAny<IngredientEntity>()))
            .Returns((IngredientEntity entity) => new IngredientDto
            {
                Id = entity.Id,
                Name = entity.Name,
                FoodEntityId = entity.FoodEntityId
            });

        _mapperMock
            .Setup(m => m.Map<IngredientEntity>(It.IsAny<CreateIngredientDto>()))
            .Returns((CreateIngredientDto dto) => new IngredientEntity
            {
                Id = 100,
                Name = dto.Name!,
                FoodEntityId = dto.FoodEntityId
            });

        _mapperMock
            .Setup(m => m.Map(It.IsAny<UpdateIngredientDto>(), It.IsAny<IngredientEntity>()))
            .Callback((UpdateIngredientDto dto, IngredientEntity entity) =>
            {
                if (!string.IsNullOrWhiteSpace(dto.Name))
                {
                    entity.Name = dto.Name;
                }

                entity.FoodEntityId = dto.FoodEntityId;
            });
    }

    [Fact]
    public async Task GetAllIngredientsAsync_ReturnsAllIngredients()
    {
        var entities = new List<IngredientEntity>
        {
            new IngredientEntity { Id = 1, Name = "Salt", FoodEntityId = 1 },
            new IngredientEntity { Id = 2, Name = "Sugar", FoodEntityId = 1 }
        };

        _repoMock.Setup(r => r.GetAll()).Returns(entities.AsQueryable());

        var result = await _service.GetAllIngredientsAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllIngredientsAsync_ReturnsEmpty_WhenNoIngredientsExist()
    {
        _repoMock.Setup(r => r.GetAll()).Returns(Enumerable.Empty<IngredientEntity>().AsQueryable());

        var result = await _service.GetAllIngredientsAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllIngredientsAsync_AppliesPagination()
    {
        var entities = new List<IngredientEntity>
        {
            new IngredientEntity { Id = 1, Name = "Apple", FoodEntityId = 1 },
            new IngredientEntity { Id = 2, Name = "Banana", FoodEntityId = 1 },
            new IngredientEntity { Id = 3, Name = "Carrot", FoodEntityId = 1 }
        };
        _repoMock.Setup(r => r.GetAll()).Returns(entities.AsQueryable());

        var result = await _service.GetAllIngredientsAsync(new QueryParameters { Page = 2, PageCount = 2 });

        Assert.Single(result);
        Assert.Equal("Carrot", result.First().Name);
    }

    [Fact]
    public async Task GetIngredientByIdAsync_ReturnsIngredient_WhenFound()
    {
        _repoMock.Setup(r => r.GetSingle(2)).Returns(new IngredientEntity { Id = 2, Name = "Sugar", FoodEntityId = 1 });

        var result = await _service.GetIngredientByIdAsync(2);

        Assert.NotNull(result);
        Assert.Equal("Sugar", result!.Name);
    }

    [Fact]
    public async Task GetIngredientByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

        var result = await _service.GetIngredientByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetIngredientByIdAsync_ReturnsNull_ForNegativeId()
    {
        var result = await _service.GetIngredientByIdAsync(-1);

        Assert.Null(result);
        _repoMock.Verify(r => r.GetSingle(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task CreateIngredientAsync_CreatesIngredientSuccessfully()
    {
        var createDto = new CreateIngredientDto { Name = "Pepper", FoodEntityId = 1 };
        _repoMock.Setup(r => r.GetAll()).Returns(Enumerable.Empty<IngredientEntity>().AsQueryable());
        _repoMock.Setup(r => r.Add(It.IsAny<IngredientEntity>()));
        _repoMock.Setup(r => r.Save()).Returns(true);
        _repoMock.Setup(r => r.GetSingle(100)).Returns(new IngredientEntity { Id = 100, Name = "Pepper", FoodEntityId = 1 });

        var result = await _service.CreateIngredientAsync(createDto);

        Assert.Equal("Pepper", result.Name);
    }

    [Fact]
    public async Task CreateIngredientAsync_Throws_WhenNameIsNull()
    {
        var createDto = new CreateIngredientDto { Name = null, FoodEntityId = 1 };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateIngredientAsync(createDto));
    }

    [Fact]
    public async Task CreateIngredientAsync_Throws_WhenDuplicateNameExists()
    {
        _repoMock.Setup(r => r.GetAll()).Returns(new[]
        {
            new IngredientEntity { Id = 1, Name = "Salt", FoodEntityId = 1 }
        }.AsQueryable());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.CreateIngredientAsync(new CreateIngredientDto { Name = "salt", FoodEntityId = 2 }));
    }

    [Fact]
    public async Task CreateIngredientAsync_MapsValuesCorrectly()
    {
        var createDto = new CreateIngredientDto { Name = " Paprika ", FoodEntityId = 3 };
        IngredientEntity? addedEntity = null;

        _repoMock.Setup(r => r.GetAll()).Returns(Enumerable.Empty<IngredientEntity>().AsQueryable());
        _repoMock.Setup(r => r.Add(It.IsAny<IngredientEntity>())).Callback<IngredientEntity>(entity => addedEntity = entity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _repoMock.Setup(r => r.GetSingle(100)).Returns(new IngredientEntity { Id = 100, Name = "Paprika", FoodEntityId = 3 });

        var result = await _service.CreateIngredientAsync(createDto);

        Assert.NotNull(addedEntity);
        Assert.Equal("Paprika", addedEntity!.Name);
        Assert.Equal(3, addedEntity.FoodEntityId);
        Assert.Equal("Paprika", result.Name);
    }

    [Fact]
    public async Task UpdateIngredientAsync_ReturnsUpdatedIngredient_WhenFound()
    {
        var existing = new IngredientEntity { Id = 4, Name = "Salt", FoodEntityId = 1 };
        _repoMock.Setup(r => r.GetSingle(4)).Returns(existing);
        _repoMock.Setup(r => r.Update(4, existing)).Returns(existing);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.UpdateIngredientAsync(4, new UpdateIngredientDto { Name = "Sea Salt", FoodEntityId = 1 });

        Assert.NotNull(result);
        Assert.Equal("Sea Salt", result!.Name);
    }

    [Fact]
    public async Task UpdateIngredientAsync_ReturnsNull_WhenIngredientNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(404)).Returns((IngredientEntity?)null);

        var result = await _service.UpdateIngredientAsync(404, new UpdateIngredientDto { Name = "Salt", FoodEntityId = 1 });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateIngredientAsync_SupportsPartialUpdateBehavior()
    {
        var existing = new IngredientEntity { Id = 5, Name = "Brown Sugar", FoodEntityId = 7 };
        _repoMock.Setup(r => r.GetSingle(5)).Returns(existing);
        _repoMock.Setup(r => r.Update(5, existing)).Returns(existing);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.UpdateIngredientAsync(5, new UpdateIngredientDto { Name = "Dark Brown Sugar", FoodEntityId = 7 });

        Assert.NotNull(result);
        Assert.Equal("Dark Brown Sugar", result!.Name);
        Assert.Equal(7, result.FoodEntityId);
    }

    [Fact]
    public async Task DeleteIngredientAsync_ReturnsTrue_WhenDeleted()
    {
        _repoMock.Setup(r => r.GetSingle(5)).Returns(new IngredientEntity { Id = 5, Name = "Flour", FoodEntityId = 1 });
        _repoMock.Setup(r => r.Delete(5));
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.DeleteIngredientAsync(5);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteIngredientAsync_ReturnsFalse_WhenIngredientNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(123)).Returns((IngredientEntity?)null);

        var result = await _service.DeleteIngredientAsync(123);

        Assert.False(result);
    }

    [Fact]
    public async Task SearchIngredientsByNameAsync_ReturnsMatches()
    {
        _repoMock.Setup(r => r.SearchIngredientsByName("sal")).Returns(new[]
        {
            new IngredientEntity { Id = 1, Name = "Salt", FoodEntityId = 1 }
        });

        var result = await _service.SearchIngredientsByNameAsync("sal");

        Assert.Single(result);
        Assert.Equal("Salt", result.First().Name);
    }

    [Fact]
    public async Task SearchIngredientsByNameAsync_ReturnsEmpty_WhenNoMatchesFound()
    {
        _repoMock.Setup(r => r.SearchIngredientsByName("zzz")).Returns(Array.Empty<IngredientEntity>());

        var result = await _service.SearchIngredientsByNameAsync("zzz");

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchIngredientsByNameAsync_IsCaseInsensitive()
    {
        _repoMock.Setup(r => r.SearchIngredientsByName("SALT")).Returns(new[]
        {
            new IngredientEntity { Id = 1, Name = "Salt", FoodEntityId = 1 }
        });

        var result = await _service.SearchIngredientsByNameAsync("SALT");

        Assert.Single(result);
        Assert.Equal("Salt", result.First().Name);
    }
}