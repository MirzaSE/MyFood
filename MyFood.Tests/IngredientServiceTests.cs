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
    private readonly IMapper _mapper;
    private readonly IngredientService _service;

    public IngredientServiceTests()
    {
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<FoodMappings>());
        _mapper = mapperConfig.CreateMapper();
        _service = new IngredientService(_repoMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAll()
    {
        _repoMock.Setup(r => r.GetAll()).Returns(CreateIngredients(3).AsQueryable());

        var result = (await _service.GetAllAsync(new QueryParameters { Page = 1, PageCount = 10 })).ToList();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty()
    {
        _repoMock.Setup(r => r.GetAll()).Returns(new List<IngredientEntity>().AsQueryable());

        var result = await _service.GetAllAsync(new QueryParameters());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_PaginationWorks()
    {
        _repoMock.Setup(r => r.GetAll()).Returns(CreateIngredients(5).AsQueryable());

        var result = (await _service.GetAllAsync(new QueryParameters { Page = 2, PageCount = 2 })).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("Ingredient 3", result[0].Name);
    }

    [Fact]
    public async Task GetByIdAsync_Found()
    {
        _repoMock.Setup(r => r.GetSingle(2)).Returns(CreateIngredient(2, "Paprika"));

        var result = await _service.GetByIdAsync(2);

        Assert.NotNull(result);
        Assert.Equal("Paprika", result!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_HandlesNegativeIds()
    {
        var result = await _service.GetByIdAsync(-1);

        Assert.Null(result);
        _repoMock.Verify(r => r.GetSingle(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_Success()
    {
        var dto = CreateIngredientCreateDto("Salt");
        _repoMock.Setup(r => r.GetByExactName("Salt")).Returns((IngredientEntity?)null);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.CreateAsync(dto);

        Assert.Equal("Salt", result.Name);
        _repoMock.Verify(r => r.Add(It.IsAny<IngredientEntity>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_NullNameError()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(CreateIngredientCreateDto(" ")));
    }

    [Fact]
    public async Task CreateAsync_DuplicateError()
    {
        _repoMock.Setup(r => r.GetByExactName("Salt")).Returns(CreateIngredient(1, "Salt"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(CreateIngredientCreateDto("Salt")));
    }

    [Fact]
    public async Task CreateAsync_MapsCorrectly()
    {
        var dto = CreateIngredientCreateDto("Oats");
        IngredientEntity? captured = null;
        _repoMock.Setup(r => r.GetByExactName("Oats")).Returns((IngredientEntity?)null);
        _repoMock.Setup(r => r.Add(It.IsAny<IngredientEntity>())).Callback<IngredientEntity>(entity => captured = entity);
        _repoMock.Setup(r => r.Save()).Returns(true);

        await _service.CreateAsync(dto);

        Assert.NotNull(captured);
        Assert.Equal("gram", captured!.Unit);
        Assert.Equal(389, captured.CaloriesPerUnit);
    }

    [Fact]
    public async Task UpdateAsync_Success()
    {
        var existing = CreateIngredient(5, "Sugar");
        var updateDto = new IngredientUpdateDto { Name = "Brown Sugar", Unit = "gram", CaloriesPerUnit = 387, Protein = 0, Carbs = 100, Fat = 0 };
        _repoMock.Setup(r => r.GetSingle(5)).Returns(existing);
        _repoMock.Setup(r => r.GetByExactName("Brown Sugar")).Returns((IngredientEntity?)null);
        _repoMock.Setup(r => r.Update(5, existing)).Returns(existing);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.UpdateAsync(5, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Brown Sugar", result!.Name);
    }

    [Fact]
    public async Task UpdateAsync_NotFound()
    {
        _repoMock.Setup(r => r.GetSingle(404)).Returns((IngredientEntity?)null);

        var result = await _service.UpdateAsync(404, CreateIngredientUpdateDto("Any"));

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate()
    {
        var existing = CreateIngredient(9, "Milk");
        var updateDto = new IngredientUpdateDto
        {
            Name = "Milk",
            Unit = existing.Unit,
            CaloriesPerUnit = 52,
            Protein = existing.Protein,
            Carbs = existing.Carbs,
            Fat = existing.Fat
        };

        _repoMock.Setup(r => r.GetSingle(9)).Returns(existing);
        _repoMock.Setup(r => r.GetByExactName("Milk")).Returns(existing);
        _repoMock.Setup(r => r.Update(9, existing)).Returns(existing);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.UpdateAsync(9, updateDto);

        Assert.NotNull(result);
        Assert.Equal(52, result!.CaloriesPerUnit);
        Assert.Equal("ml", result.Unit);
    }

    [Fact]
    public async Task DeleteAsync_Success()
    {
        _repoMock.Setup(r => r.GetSingle(2)).Returns(CreateIngredient(2, "Salt"));
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.DeleteAsync(2);

        Assert.True(result);
        _repoMock.Verify(r => r.Delete(2), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NotFound()
    {
        _repoMock.Setup(r => r.GetSingle(2)).Returns((IngredientEntity?)null);

        var result = await _service.DeleteAsync(2);

        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_FindsMatches()
    {
        _repoMock.Setup(r => r.SearchByName("tom")).Returns(new[]
        {
            CreateIngredient(1, "Tomato"),
            CreateIngredient(2, "Tomatillo")
        }.AsQueryable());

        var result = (await _service.SearchAsync("tom", new QueryParameters { Page = 1, PageCount = 10 })).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty()
    {
        _repoMock.Setup(r => r.SearchByName("xyz")).Returns(new List<IngredientEntity>().AsQueryable());

        var result = await _service.SearchAsync("xyz", new QueryParameters());

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_CaseInsensitive()
    {
        _repoMock.Setup(r => r.SearchByName("RICE")).Returns(new[]
        {
            CreateIngredient(1, "Rice")
        }.AsQueryable());

        var result = (await _service.SearchAsync("RICE", new QueryParameters())).ToList();

        Assert.Single(result);
        Assert.Equal("Rice", result[0].Name);
    }

    private static List<IngredientEntity> CreateIngredients(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreateIngredient(i, $"Ingredient {i}"))
            .ToList();
    }

    private static IngredientEntity CreateIngredient(int id, string name)
    {
        return new IngredientEntity
        {
            Id = id,
            Name = name,
            Unit = id == 9 ? "ml" : "gram",
            CaloriesPerUnit = 100 + id,
            Protein = 10,
            Carbs = 5,
            Fat = 2
        };
    }

    private static IngredientCreateDto CreateIngredientCreateDto(string name)
    {
        return new IngredientCreateDto
        {
            Name = name,
            Unit = "gram",
            CaloriesPerUnit = 389,
            Protein = 17,
            Carbs = 66,
            Fat = 7
        };
    }

    private static IngredientUpdateDto CreateIngredientUpdateDto(string name)
    {
        return new IngredientUpdateDto
        {
            Name = name,
            Unit = "gram",
            CaloriesPerUnit = 389,
            Protein = 17,
            Carbs = 66,
            Fat = 7
        };
    }
}
