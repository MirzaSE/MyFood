using Moq;
using AutoMapper;
using MyFood.Application;
using MyFood.Application.Services;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

public class IngredientServiceTests
{
    private readonly Mock<IIngredientRepository> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly IngredientService _service;

    public IngredientServiceTests()
    {
        _service = new IngredientService(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMapped()
    {
        var entities = new List<IngredientEntity> { new IngredientEntity { Id = 1, Name = "Salt" } };
        var dtos = new List<IngredientDto> { new IngredientDto { Id = 1, Name = "Salt" } };
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.GetAllAsync(new QueryParameters());
        Assert.Single(result);
        Assert.Equal("Salt", result.First().Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoItems()
    {
        var entities = new List<IngredientEntity>();
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto>());

        var result = await _service.GetAllAsync(new QueryParameters());
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMapped_WhenFound()
    {
        var entity = new IngredientEntity { Id = 2, Name = "Pepper" };
        var dto = new IngredientDto { Id = 2, Name = "Pepper" };
        _repoMock.Setup(r => r.GetSingle(2)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.GetByIdAsync(2);
        Assert.NotNull(result);
        Assert.Equal("Pepper", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity)null);
        var result = await _service.GetByIdAsync(99);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_AddsAndReturnsDto()
    {
        var createDto = new IngredientCreateDto { Name = "Sugar" };
        var entity = new IngredientEntity { Id = 3, Name = "Sugar" };
        var dto = new IngredientDto { Id = 3, Name = "Sugar" };
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Add(entity));
        _repoMock.Setup(r => r.Save()).Returns(true);
        _repoMock.Setup(r => r.GetSingle(entity.Id)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.CreateAsync(createDto);
        Assert.Equal("Sugar", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesAndReturnsDto()
    {
        var updateDto = new IngredientUpdateDto { Name = "Updated" };
        var entity = new IngredientEntity { Id = 4, Name = "Old" };
        var updatedEntity = new IngredientEntity { Id = 4, Name = "Updated" };
        var dto = new IngredientDto { Id = 4, Name = "Updated" };
        _repoMock.Setup(r => r.GetSingle(4)).Returns(entity);
        _mapperMock.Setup(m => m.Map(updateDto, entity));
        _repoMock.Setup(r => r.Update(4, entity)).Returns(updatedEntity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(updatedEntity)).Returns(dto);

        var result = await _service.UpdateAsync(4, updateDto);
        Assert.NotNull(result);
        Assert.Equal("Updated", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity)null);
        var result = await _service.UpdateAsync(99, new IngredientUpdateDto());
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_DeletesAndReturnsTrue()
    {
        var entity = new IngredientEntity { Id = 5 };
        _repoMock.Setup(r => r.GetSingle(5)).Returns(entity);
        _repoMock.Setup(r => r.Delete(5));
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.DeleteAsync(5);
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity)null);
        var result = await _service.DeleteAsync(99);
        Assert.False(result);
    }
}
