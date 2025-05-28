using Xunit;
using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
using MyFood.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using MyFood.Application.Helpers;
using MyFood.Infrastructure;

public class FoodServiceTests
{
    private readonly Mock<IFoodRepository> _foodRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILinkService<FoodService>> _linkServiceMock;
    private readonly FoodService _service;

    public FoodServiceTests()
    {
        _foodRepoMock = new Mock<IFoodRepository>();
        _mapperMock = new Mock<IMapper>();
        _linkServiceMock = new Mock<ILinkService<FoodService>>();
        _service = new FoodService(_foodRepoMock.Object, _mapperMock.Object, _linkServiceMock.Object);
    }

    [Fact]
    public void GetSingleFood_ReturnsFood_WhenIdIsValid()
    {
        // Arrange
        var id = 1;
        var version = new ApiVersion(1, 0);
        var entity = new FoodEntity { Id = id };
        var dto = new FoodDto { Id = id };
        var expectedResult = new object();

        _foodRepoMock.Setup(r => r.GetSingle(id)).Returns(entity);
        _mapperMock.Setup(m => m.Map<FoodDto>(entity)).Returns(dto);
        _linkServiceMock.Setup(l => l.ExpandSingleFoodItem(dto, dto.Id, version)).Returns(expectedResult);

        // Act
        var result = _service.GetSingleFood(id, version);

        // Assert
        Assert.False(result.NotFound);
        Assert.Equal(expectedResult, result.Food);
    }

    [Fact]
    public void GetSingleFood_ReturnsNotFound_WhenIdIsInvalid()
    {
        var id = -1;
        var result = _service.GetSingleFood(id, new ApiVersion(1, 0));

        Assert.True(result.NotFound);
        Assert.Null(result.Food);
    }

    [Fact]
    public void AddFood_ReturnsCreatedObject_WhenInputIsValid()
    {
        var createDto = new FoodCreateDto();
        var entity = new FoodEntity { Id = 1 };
        var savedEntity = new FoodEntity { Id = 1 };
        var dto = new FoodDto { Id = 1 };
        var version = new ApiVersion(1, 0);
        var expectedResult = new object();

        _mapperMock.Setup(m => m.Map<FoodEntity>(createDto)).Returns(entity);
        _foodRepoMock.Setup(r => r.Add(entity));
        _foodRepoMock.Setup(r => r.Save()).Returns(true);
        _foodRepoMock.Setup(r => r.GetSingle(entity.Id)).Returns(savedEntity);
        _mapperMock.Setup(m => m.Map<FoodDto>(savedEntity)).Returns(dto);
        _linkServiceMock.Setup(l => l.ExpandSingleFoodItem(dto, dto.Id, version)).Returns(expectedResult);

        var result = _service.AddFood(createDto, version);

        Assert.True(result.Created);
        Assert.Equal(expectedResult, result.Food);
        Assert.Null(result.Error);
    }

    [Fact]
    public void AddFood_ReturnsError_WhenSaveFails()
    {
        var createDto = new FoodCreateDto();
        var entity = new FoodEntity();

        _mapperMock.Setup(m => m.Map<FoodEntity>(createDto)).Returns(entity);
        _foodRepoMock.Setup(r => r.Add(entity));
        _foodRepoMock.Setup(r => r.Save()).Returns(false);

        var result = _service.AddFood(createDto, new ApiVersion(1, 0));

        Assert.False(result.Created);
        Assert.Equal("Creating a fooditem failed on save.", result.Error);
    }

    [Fact]
    public void RemoveFood_ReturnsNotFound_WhenIdIsInvalid()
    {
        _foodRepoMock.Setup(r => r.GetSingle(99)).Returns((FoodEntity)null);

        var result = _service.RemoveFood(99);

        Assert.True(result.NotFound);
        Assert.Null(result.Error);
    }

    [Fact]
    public void RemoveFood_ReturnsSuccess_WhenDeleted()
    {
        var id = 1;
        _foodRepoMock.Setup(r => r.GetSingle(id)).Returns(new FoodEntity { Id = id });
        _foodRepoMock.Setup(r => r.Save()).Returns(true);

        var result = _service.RemoveFood(id);

        Assert.False(result.NotFound);
        Assert.Null(result.Error);
    }

    [Fact]
    public void RemoveFood_ReturnsError_WhenSaveFails()
    {
        var id = 2;
        _foodRepoMock.Setup(r => r.GetSingle(id)).Returns(new FoodEntity { Id = id });
        _foodRepoMock.Setup(r => r.Save()).Returns(false);

        var result = _service.RemoveFood(id);

        Assert.False(result.NotFound);
        Assert.Equal("Deleting a fooditem failed on save.", result.Error);
    }
}
