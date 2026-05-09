using Moq;
using AutoMapper;
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
    }

    // ========== GetAllAsync Tests ==========

    [Fact]
    public async Task GetAllAsync_ReturnsAllIngredients()
    {
        var entities = new List<IngredientEntity>
        {
            new() { Id = 1, Name = "Salt" },
            new() { Id = 2, Name = "Pepper" }
        };
        var dtos = new List<IngredientDto>
        {
            new() { Id = 1, Name = "Salt" },
            new() { Id = 2, Name = "Pepper" }
        };
        _repoMock.Setup(r => r.GetAll()).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.GetAllAsync(new QueryParameters { Page = 1, PageCount = 50 });

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoIngredients()
    {
        var entities = new List<IngredientEntity>();
        var dtos = new List<IngredientDto>();
        _repoMock.Setup(r => r.GetAll()).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.GetAllAsync(new QueryParameters());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_PaginationWorks()
    {
        var entities = new List<IngredientEntity>();
        for (int i = 1; i <= 10; i++)
            entities.Add(new IngredientEntity { Id = i, Name = $"Ingredient{i}" });

        _repoMock.Setup(r => r.GetAll()).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns((IEnumerable<IngredientEntity> src) =>
                src.Select(e => new IngredientDto { Id = e.Id, Name = e.Name }));

        var result = await _service.GetAllAsync(new QueryParameters { Page = 1, PageCount = 5 });

        Assert.Equal(5, result.Count());
    }

    // ========== GetByIdAsync Tests ==========

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenFound()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Sugar" };
        var dto = new IngredientDto { Id = 1, Name = "Sugar" };
        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Sugar", result!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_HandlesNegativeIds()
    {
        _repoMock.Setup(r => r.GetSingle(-1)).Returns((IngredientEntity?)null);

        var result = await _service.GetByIdAsync(-1);

        Assert.Null(result);
    }

    // ========== CreateAsync Tests ==========

    [Fact]
    public async Task CreateAsync_Success_ReturnsDto()
    {
        var createDto = new IngredientCreateDto { Name = "Flour", Quantity = "500g", FoodId = 1 };
        var entity = new IngredientEntity { Id = 1, Name = "Flour" };
        var dto = new IngredientDto { Id = 1, Name = "Flour" };

        _repoMock.Setup(r => r.GetAll()).Returns(new List<IngredientEntity>().AsQueryable());
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Add(entity));
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.CreateAsync(createDto);

        Assert.Equal("Flour", result.Name);
    }

    [Fact]
    public async Task CreateAsync_NullName_ThrowsArgumentException()
    {
        var createDto = new IngredientCreateDto { Name = null, Quantity = "100g", FoodId = 1 };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_DuplicateName_ThrowsInvalidOperationException()
    {
        var existing = new List<IngredientEntity>
        {
            new() { Id = 1, Name = "Salt" }
        };
        _repoMock.Setup(r => r.GetAll()).Returns(existing.AsQueryable());

        var createDto = new IngredientCreateDto { Name = "Salt", Quantity = "10g", FoodId = 1 };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_MapsCorrectly()
    {
        var createDto = new IngredientCreateDto { Name = "Olive Oil", Quantity = "2tbsp", FoodId = 1 };
        var entity = new IngredientEntity { Id = 5, Name = "Olive Oil" };
        var dto = new IngredientDto { Id = 5, Name = "Olive Oil", Quantity = "2tbsp", FoodId = 1 };

        _repoMock.Setup(r => r.GetAll()).Returns(new List<IngredientEntity>().AsQueryable());
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.CreateAsync(createDto);

        Assert.Equal(5, result.Id);
        Assert.Equal("Olive Oil", result.Name);
        Assert.Equal("2tbsp", result.Quantity);
        _mapperMock.Verify(m => m.Map<IngredientEntity>(createDto), Times.Once);
    }

    // ========== UpdateAsync Tests ==========

    [Fact]
    public async Task UpdateAsync_Success_ReturnsUpdatedDto()
    {
        var updateDto = new IngredientUpdateDto { Name = "Updated Salt", Quantity = "20g", FoodId = 1 };
        var entity = new IngredientEntity { Id = 1, Name = "Salt" };
        var dto = new IngredientDto { Id = 1, Name = "Updated Salt" };

        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _mapperMock.Setup(m => m.Map(updateDto, entity));
        _repoMock.Setup(r => r.Update(entity)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.UpdateAsync(1, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Updated Salt", result!.Name);
    }

    [Fact]
    public async Task UpdateAsync_NotFound_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

        var result = await _service.UpdateAsync(99, new IngredientUpdateDto());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate_UpdatesOnlyProvidedFields()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Salt" };
        var updateDto = new IngredientUpdateDto { Name = "Sea Salt", Quantity = "5g", FoodId = 1 };
        var dto = new IngredientDto { Id = 1, Name = "Sea Salt", Quantity = "5g" };

        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _mapperMock.Setup(m => m.Map(updateDto, entity));
        _repoMock.Setup(r => r.Update(entity)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.UpdateAsync(1, updateDto);

        Assert.NotNull(result);
        _mapperMock.Verify(m => m.Map(updateDto, entity), Times.Once);
    }

    // ========== DeleteAsync Tests ==========

    [Fact]
    public async Task DeleteAsync_Success_ReturnsTrue()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Salt" };
        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _repoMock.Setup(r => r.Delete(entity));
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_NotFound_ReturnsFalse()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

        var result = await _service.DeleteAsync(99);

        Assert.False(result);
    }

    // ========== SearchAsync Tests ==========

    [Fact]
    public async Task SearchAsync_FindsMatches()
    {
        var entities = new List<IngredientEntity>
        {
            new() { Id = 1, Name = "Salt" },
            new() { Id = 2, Name = "Pepper" },
            new() { Id = 3, Name = "Salted Butter" }
        };
        _repoMock.Setup(r => r.GetAll()).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns((IEnumerable<IngredientEntity> src) =>
                src.Select(e => new IngredientDto { Id = e.Id, Name = e.Name }));

        var result = await _service.SearchAsync("salt");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatch()
    {
        var entities = new List<IngredientEntity>
        {
            new() { Id = 1, Name = "Salt" },
            new() { Id = 2, Name = "Pepper" }
        };
        _repoMock.Setup(r => r.GetAll()).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns(new List<IngredientDto>());

        var result = await _service.SearchAsync("xyz");

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_IsCaseInsensitive()
    {
        var entities = new List<IngredientEntity>
        {
            new() { Id = 1, Name = "Olive Oil" },
            new() { Id = 2, Name = "OLIVE paste" }
        };
        _repoMock.Setup(r => r.GetAll()).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns((IEnumerable<IngredientEntity> src) =>
                src.Select(e => new IngredientDto { Id = e.Id, Name = e.Name }));

        var result = await _service.SearchAsync("OLIVE");

        Assert.Equal(2, result.Count());
    }
}
