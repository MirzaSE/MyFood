using Moq;
using AutoMapper;
using MyFood.Application.Services;
using MyFood.Application.Dtos;
using MyFood.Application;
using MyFood.Domain.Entities;

public class FoodServiceTests
{
    private readonly Mock<IFoodRepository> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private readonly FoodService _service;

    public FoodServiceTests()
    {
        _service = new FoodService(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllFoodsAsync_ReturnsMappedDtos()
    {

        var entities = new List<FoodEntity> { new FoodEntity { Id = 1, Name = "Apple" } };
        var dtos = new List<FoodDto> { new FoodDto { Id = 1, Name = "Apple" } };
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<FoodDto>>(It.IsAny<IEnumerable<FoodEntity>>())).Returns(dtos);

        var result = await _service.GetAllFoodsAsync(new QueryParameters());
        Assert.Single(result);
        Assert.Equal("Apple", result.First().Name);
    }

    [Fact]
    public async Task GetFoodByIdAsync_ReturnsMappedDto_WhenFound()
    {
        var entity = new FoodEntity { Id = 2, Name = "Banana" };
        var dto = new FoodDto { Id = 2, Name = "Banana" };
        _repoMock.Setup(r => r.GetSingle(2)).Returns(entity);
        _mapperMock.Setup(m => m.Map<FoodDto>(entity)).Returns(dto);

        var result = await _service.GetFoodByIdAsync(2);
        Assert.NotNull(result);
        Assert.Equal("Banana", result.Name);
    }

    [Fact]
    public async Task GetFoodByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((FoodEntity)null);
        var result = await _service.GetFoodByIdAsync(99);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateFoodAsync_AddsAndReturnsDto()
    {
        var createDto = new FoodCreateDto { Name = "Pear", Type = "Fruit" };
        var entity = new FoodEntity { Id = 3, Name = "Pear" };
        var dto = new FoodDto { Id = 3, Name = "Pear" };
        _mapperMock.Setup(m => m.Map<FoodEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Add(entity));
        _repoMock.Setup(r => r.Save()).Returns(true);
        _repoMock.Setup(r => r.GetSingle(entity.Id)).Returns(entity);
        _mapperMock.Setup(m => m.Map<FoodDto>(entity)).Returns(dto);

        var result = await _service.CreateFoodAsync(createDto);
        Assert.Equal("Pear", result.Name);
    }

    [Fact]
    public async Task UpdateFoodAsync_UpdatesAndReturnsDto()
    {
        var updateDto = new FoodUpdateDto { Name = "Updated", Type = "Fruit" };
        var entity = new FoodEntity { Id = 4, Name = "Old" };
        var updatedEntity = new FoodEntity { Id = 4, Name = "Updated" };
        var dto = new FoodDto { Id = 4, Name = "Updated" };
        _repoMock.Setup(r => r.GetSingle(4)).Returns(entity);
        _mapperMock.Setup(m => m.Map(updateDto, entity));
        _repoMock.Setup(r => r.Update(4, entity)).Returns(updatedEntity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<FoodDto>(updatedEntity)).Returns(dto);

        var result = await _service.UpdateFoodAsync(4, updateDto);
        Assert.NotNull(result);
        Assert.Equal("Updated", result.Name);
    }

    [Fact]
    public async Task UpdateFoodAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((FoodEntity)null);
        var result = await _service.UpdateFoodAsync(99, new FoodUpdateDto { Name = "Any", Type = "Any" });
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
        _repoMock.Setup(r => r.GetSingle(99)).Returns((FoodEntity)null);
        var result = await _service.DeleteFoodAsync(99);
        Assert.False(result);
    }
}
