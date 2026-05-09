using Moq;
using AutoMapper;
using MyFood.Application.Services;
using MyFood.Application.Dtos;
using MyFood.Application;
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

    // ==================== GetAllAsync ====================

    [Fact]
    public async Task GetAllAsync_ReturnsAllIngredients()
    {
        var entities = new List<IngredientEntity>
        {
            new() { Id = 1, Name = "Salt", Unit = "g", CaloriesPerUnit = 0, Protein = 0, Carbs = 0, Fat = 0 },
            new() { Id = 2, Name = "Sugar", Unit = "g", CaloriesPerUnit = 4, Protein = 0, Carbs = 1, Fat = 0 }
        };
        var dtos = new List<IngredientDto>
        {
            new() { Id = 1, Name = "Salt" },
            new() { Id = 2, Name = "Sugar" }
        };

        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.GetAllAsync(new QueryParameters());

        Assert.Equal(2, result.Count());
        Assert.Contains(result, i => i.Name == "Salt");
        Assert.Contains(result, i => i.Name == "Sugar");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoIngredients()
    {
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(new List<IngredientEntity>().AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns(new List<IngredientDto>());

        var result = await _service.GetAllAsync(new QueryParameters());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_PaginationWorks()
    {
        var entities = new List<IngredientEntity> { new() { Id = 1, Name = "Salt" } };
        var dtos = new List<IngredientDto> { new() { Id = 1, Name = "Salt" } };

        var qp = new QueryParameters { Page = 2, PageCount = 5 };
        _repoMock.Setup(r => r.GetAll(qp)).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.GetAllAsync(qp);

        _repoMock.Verify(r => r.GetAll(qp), Times.Once);
        Assert.Single(result);
    }

    // ==================== GetByIdAsync ====================

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenFound()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Salt", Unit = "g" };
        var dto = new IngredientDto { Id = 1, Name = "Salt", Unit = "g" };

        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Salt", result!.Name);
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
        var result = await _service.GetByIdAsync(-1);

        Assert.Null(result);
        _repoMock.Verify(r => r.GetSingle(It.IsAny<int>()), Times.Never);
    }

    // ==================== CreateAsync ====================

    [Fact]
    public async Task CreateAsync_Succeeds()
    {
        var createDto = new IngredientCreateDto
        {
            Name = "Salt", Unit = "g", CaloriesPerUnit = 0, Protein = 0, Carbs = 0, Fat = 0
        };
        var entity = new IngredientEntity { Id = 1, Name = "Salt", Unit = "g" };
        var dto = new IngredientDto { Id = 1, Name = "Salt", Unit = "g" };

        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Add(entity));
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.CreateAsync(createDto);

        Assert.Equal("Salt", result.Name);
        _repoMock.Verify(r => r.Add(entity), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenNameIsNull()
    {
        var createDto = new IngredientCreateDto { Name = "", Unit = "g" };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenNameIsNull_WhitespaceOnly()
    {
        var createDto = new IngredientCreateDto { Name = "   ", Unit = "g" };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenUnitIsNull()
    {
        var createDto = new IngredientCreateDto { Name = "Salt", Unit = "" };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_MapsCorrectly()
    {
        var createDto = new IngredientCreateDto
        {
            Name = "Flour", Unit = "cup", CaloriesPerUnit = 455, Protein = 13, Carbs = 95, Fat = 1
        };
        var entity = new IngredientEntity { Id = 10, Name = "Flour", Unit = "cup" };
        var dto = new IngredientDto { Id = 10, Name = "Flour", Unit = "cup", CaloriesPerUnit = 455 };

        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.CreateAsync(createDto);

        Assert.Equal(10, result.Id);
        Assert.Equal("Flour", result.Name);
        Assert.Equal("cup", result.Unit);
        Assert.Equal(455, result.CaloriesPerUnit);
    }

    // ==================== UpdateAsync ====================

    [Fact]
    public async Task UpdateAsync_Succeeds()
    {
        var updateDto = new IngredientUpdateDto { Name = "Pepper" };
        var existing = new IngredientEntity { Id = 1, Name = "Salt" };
        var dto = new IngredientDto { Id = 1, Name = "Pepper" };

        _repoMock.Setup(r => r.GetSingle(1)).Returns(existing);
        _mapperMock.Setup(m => m.Map(updateDto, existing));
        _repoMock.Setup(r => r.Update(1, existing)).Returns(existing);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(existing)).Returns(dto);

        var result = await _service.UpdateAsync(1, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Pepper", result!.Name);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

        var result = await _service.UpdateAsync(99, new IngredientUpdateDto { Name = "X" });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate_OnlyChangesProvidedFields()
    {
        var updateDto = new IngredientUpdateDto { CaloriesPerUnit = 100 };
        var existing = new IngredientEntity { Id = 1, Name = "Salt", CaloriesPerUnit = 0 };
        var dto = new IngredientDto { Id = 1, Name = "Salt", CaloriesPerUnit = 100 };

        _repoMock.Setup(r => r.GetSingle(1)).Returns(existing);
        _mapperMock.Setup(m => m.Map(updateDto, existing));
        _repoMock.Setup(r => r.Update(1, existing)).Returns(existing);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(existing)).Returns(dto);

        var result = await _service.UpdateAsync(1, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Salt", result!.Name);
        Assert.Equal(100, result.CaloriesPerUnit);
    }

    // ==================== DeleteAsync ====================

    [Fact]
    public async Task DeleteAsync_Succeeds()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Salt" };
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

    // ==================== SearchAsync ====================

    [Fact]
    public async Task SearchAsync_FindsMatches()
    {
        var entities = new List<IngredientEntity>
        {
            new() { Id = 1, Name = "Sea Salt" },
            new() { Id = 2, Name = "Salted Butter" }
        };
        var dtos = new List<IngredientDto>
        {
            new() { Id = 1, Name = "Sea Salt" },
            new() { Id = 2, Name = "Salted Butter" }
        };

        _repoMock.Setup(r => r.SearchIngredientByName("salt")).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

        var result = await _service.SearchAsync("salt");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
    {
        _repoMock.Setup(r => r.SearchIngredientByName("xyz")).Returns(new List<IngredientEntity>());
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
            new() { Id = 1, Name = "SALT" }
        };
        var dtos = new List<IngredientDto> { new() { Id = 1, Name = "SALT" } };

        _repoMock.Setup(r => r.SearchIngredientByName("salt")).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

        var result = await _service.SearchAsync("salt");

        Assert.Single(result);
        Assert.Equal("SALT", result.First().Name);
    }
}
