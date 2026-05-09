using AutoMapper;
using Moq;
using MyFood.Api.MappingProfiles;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

public class FoodServiceTests
{
    private readonly Mock<IFoodRepository> _repoMock = new();
    private readonly Mock<IIngredientRepository> _ingredientRepoMock = new();
    private readonly IMapper _mapper;
    private readonly FoodService _service;

    public FoodServiceTests()
    {
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<FoodMappings>());
        _mapper = mapperConfig.CreateMapper();
        _service = new FoodService(_repoMock.Object, _ingredientRepoMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllFoodsAsync_ReturnsMappedDtos()
    {
        var entities = new List<FoodEntity> { new FoodEntity { Id = 1, Name = "Apple", Type = "Fruit", Calories = 95 } };
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());

        var result = (await _service.GetAllFoodsAsync(new QueryParameters())).ToList();

        Assert.Single(result);
        Assert.Equal("Apple", result[0].Name);
    }

    [Fact]
    public async Task GetFoodByIdAsync_ReturnsMappedDto_WhenFound()
    {
        var entity = new FoodEntity { Id = 2, Name = "Banana", Type = "Fruit", Calories = 105 };
        _repoMock.Setup(r => r.GetSingle(2)).Returns(entity);

        var result = await _service.GetFoodByIdAsync(2);

        Assert.NotNull(result);
        Assert.Equal("Banana", result!.Name);
    }

    [Fact]
    public async Task GetFoodByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((FoodEntity?)null);

        var result = await _service.GetFoodByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateFoodAsync_AddsAndReturnsDto()
    {
        var createDto = new FoodCreateDto { Name = "Pear", Type = "Fruit", Calories = 40 };
        var entity = new FoodEntity { Id = 3, Name = "Pear", Type = "Fruit", Calories = 40 };
        _repoMock.Setup(r => r.Add(It.IsAny<FoodEntity>()))
            .Callback<FoodEntity>(food => food.Id = entity.Id);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _repoMock.Setup(r => r.GetSingle(entity.Id)).Returns(entity);

        var result = await _service.CreateFoodAsync(createDto);

        Assert.Equal("Pear", result.Name);
    }

    [Fact]
    public async Task UpdateFoodAsync_UpdatesAndReturnsDto()
    {
        var updateDto = new FoodUpdateDto { Name = "Updated", Type = "Main", Calories = 500 };
        var entity = new FoodEntity { Id = 4, Name = "Old", Type = "Starter", Calories = 100 };
        var updatedEntity = new FoodEntity { Id = 4, Name = "Updated", Type = "Main", Calories = 500 };
        _repoMock.Setup(r => r.GetSingle(4)).Returns(entity);
        _repoMock.Setup(r => r.Update(4, entity)).Returns(updatedEntity);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.UpdateFoodAsync(4, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Updated", result!.Name);
    }

    [Fact]
    public async Task UpdateFoodAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((FoodEntity?)null);

        var result = await _service.UpdateFoodAsync(99, new FoodUpdateDto { Name = "X", Type = "Y", Calories = 1 });

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteFoodAsync_DeletesAndReturnsTrue()
    {
        var entity = new FoodEntity { Id = 5 };
        _repoMock.Setup(r => r.GetSingle(5)).Returns(entity);
        _repoMock.Setup(r => r.Delete(5));
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.DeleteFoodAsync(5);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteFoodAsync_ReturnsFalse_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((FoodEntity?)null);

        var result = await _service.DeleteFoodAsync(99);

        Assert.False(result);
    }
}
