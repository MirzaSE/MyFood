using AutoMapper;
using Moq;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Tests;

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
    public async Task GetAllAsync_ReturnsAllIngredients()
    {
        var entities = new List<IngredientEntity> { new() { Id = 1, Name = "Rice", Unit = "g" } };
        var dtos = new List<IngredientDto> { new() { Id = 1, Name = "Rice", Unit = "g" } };
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.GetAllAsync(new QueryParameters());
        Assert.Single(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoIngredients()
    {
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(new List<IngredientEntity>().AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto>());

        var result = await _service.GetAllAsync(new QueryParameters());
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_PaginationWorks()
    {
        var qp = new QueryParameters { Page = 2, PageCount = 5 };
        _repoMock.Setup(r => r.GetAll(qp)).Returns(new List<IngredientEntity>().AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto>());

        await _service.GetAllAsync(qp);
        _repoMock.Verify(r => r.GetAll(qp), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Found_ReturnsIngredient()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Milk", Unit = "ml" };
        var dto = new IngredientDto { Id = 1, Name = "Milk", Unit = "ml" };
        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.GetByIdAsync(1);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetSingle(123)).Returns((IngredientEntity?)null);
        var result = await _service.GetByIdAsync(123);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_NegativeId_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(-3);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_Success_ReturnsMappedIngredient()
    {
        var create = new IngredientCreateDto { Name = "Egg", Unit = "pcs", CaloriesPerUnit = 70 };
        var entity = new IngredientEntity { Id = 5, Name = "Egg", Unit = "pcs", CaloriesPerUnit = 70 };
        var dto = new IngredientDto { Id = 5, Name = "Egg", Unit = "pcs", CaloriesPerUnit = 70 };
        _repoMock.Setup(r => r.ExistsByName("Egg", null)).Returns(false);
        _mapperMock.Setup(m => m.Map<IngredientEntity>(create)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.CreateAsync(create);
        Assert.Equal(5, result.Id);
    }

    [Fact]
    public async Task CreateAsync_NullName_ThrowsError()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateAsync(new IngredientCreateDto { Name = "", Unit = "g", CaloriesPerUnit = 1 }));
    }

    [Fact]
    public async Task CreateAsync_Duplicate_ThrowsError()
    {
        _repoMock.Setup(r => r.ExistsByName("Sugar", null)).Returns(true);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.CreateAsync(new IngredientCreateDto { Name = "Sugar", Unit = "g", CaloriesPerUnit = 4 }));
    }

    [Fact]
    public async Task CreateAsync_MapsCorrectly()
    {
        var create = new IngredientCreateDto { Name = "Salt", Unit = "g", CaloriesPerUnit = 0 };
        _repoMock.Setup(r => r.ExistsByName("Salt", null)).Returns(false);
        _mapperMock.Setup(m => m.Map<IngredientEntity>(create)).Returns(new IngredientEntity { Name = "Salt", Unit = "g" });
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(It.IsAny<IngredientEntity>())).Returns(new IngredientDto { Name = "Salt" });

        await _service.CreateAsync(create);
        _mapperMock.Verify(m => m.Map<IngredientEntity>(create), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Success_ReturnsUpdated()
    {
        var entity = new IngredientEntity { Id = 10, Name = "Yogurt", Unit = "ml", CaloriesPerUnit = 1 };
        _repoMock.Setup(r => r.GetSingle(10)).Returns(entity);
        _repoMock.Setup(r => r.ExistsByName("Greek Yogurt", 10)).Returns(false);
        _repoMock.Setup(r => r.Update(entity)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(new IngredientDto { Id = 10, Name = "Greek Yogurt" });

        var result = await _service.UpdateAsync(10, new IngredientUpdateDto { Name = "Greek Yogurt" });
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_NotFound_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetSingle(404)).Returns((IngredientEntity?)null);
        var result = await _service.UpdateAsync(404, new IngredientUpdateDto { Name = "X" });
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate_OnlyProvidedFieldsChanged()
    {
        var entity = new IngredientEntity { Id = 2, Name = "Butter", Unit = "g", CaloriesPerUnit = 7, Protein = 1, Carbs = 1, Fat = 80 };
        _repoMock.Setup(r => r.GetSingle(2)).Returns(entity);
        _repoMock.Setup(r => r.Update(entity)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(new IngredientDto { Id = 2, Name = "Butter", Unit = "tbsp" });

        await _service.UpdateAsync(2, new IngredientUpdateDto { Unit = "tbsp" });
        Assert.Equal("Butter", entity.Name);
        Assert.Equal("tbsp", entity.Unit);
    }

    [Fact]
    public async Task DeleteAsync_Success_ReturnsTrue()
    {
        var entity = new IngredientEntity { Id = 3, Name = "Tomato", Unit = "g" };
        _repoMock.Setup(r => r.GetSingle(3)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.DeleteAsync(3);
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_NotFound_ReturnsFalse()
    {
        _repoMock.Setup(r => r.GetSingle(999)).Returns((IngredientEntity?)null);
        var result = await _service.DeleteAsync(999);
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_FindsMatches()
    {
        _repoMock.Setup(r => r.SearchByName("rice")).Returns(new List<IngredientEntity> { new() { Id = 1, Name = "Rice", Unit = "g" } });
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto> { new() { Id = 1, Name = "Rice" } });
        var result = await _service.SearchAsync("rice");
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty()
    {
        _repoMock.Setup(r => r.SearchByName("zz")).Returns(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto>());
        var result = await _service.SearchAsync("zz");
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_CaseInsensitive()
    {
        _repoMock.Setup(r => r.SearchByName("RICE")).Returns(new List<IngredientEntity> { new() { Name = "Rice", Unit = "g" } });
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto> { new() { Name = "Rice" } });
        var result = await _service.SearchAsync("RICE");
        Assert.Single(result);
    }
}
