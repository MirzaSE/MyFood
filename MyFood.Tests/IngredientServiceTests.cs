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

    // ─── GetAllAsync (3 tests) ────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtos()
    {
        var entities = new List<IngredientEntity>
        {
            new IngredientEntity { Id = 1, Name = "Salt", Unit = "grams", CaloriesPerUnit = 0 }
        };
        var dtos = new List<IngredientDto>
        {
            new IngredientDto { Id = 1, Name = "Salt", Unit = "grams", CaloriesPerUnit = 0 }
        };
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.GetAllAsync(new QueryParameters());

        Assert.Single(result);
        Assert.Equal("Salt", result.First().Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoIngredients()
    {
        var entities = new List<IngredientEntity>();
        var dtos = new List<IngredientDto>();
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.GetAllAsync(new QueryParameters());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_PaginationWorks()
    {
        var queryParameters = new QueryParameters { Page = 2, PageCount = 5 };
        var entities = new List<IngredientEntity>();
        var dtos = new List<IngredientDto>();
        _repoMock.Setup(r => r.GetAll(queryParameters)).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        await _service.GetAllAsync(queryParameters);

        _repoMock.Verify(r => r.GetAll(queryParameters), Times.Once);
    }

    // ─── GetByIdAsync (3 tests) ───────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedDto_WhenFound()
    {
        var entity = new IngredientEntity { Id = 2, Name = "Pepper", Unit = "grams" };
        var dto = new IngredientDto { Id = 2, Name = "Pepper", Unit = "grams" };
        _repoMock.Setup(r => r.GetSingle(2)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.GetByIdAsync(2);

        Assert.NotNull(result);
        Assert.Equal("Pepper", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_HandlesNegativeId()
    {
        _repoMock.Setup(r => r.GetSingle(-1)).Returns((IngredientEntity?)null);

        var result = await _service.GetByIdAsync(-1);

        Assert.Null(result);
    }

    // ─── CreateAsync (4 tests) ────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_Success_ReturnsDto()
    {
        var createDto = new IngredientCreateDto { Name = "Sugar", Unit = "grams", CaloriesPerUnit = 4 };
        var entity = new IngredientEntity { Id = 3, Name = "Sugar", Unit = "grams", CaloriesPerUnit = 4 };
        var dto = new IngredientDto { Id = 3, Name = "Sugar", Unit = "grams", CaloriesPerUnit = 4 };
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Add(entity));
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.CreateAsync(createDto);

        Assert.Equal("Sugar", result.Name);
        Assert.Equal(4, result.CaloriesPerUnit);
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_WhenNameIsNull()
    {
        var createDto = new IngredientCreateDto { Name = null, Unit = "grams" };
        var entity = new IngredientEntity { Name = string.Empty };
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Add(entity));
        _repoMock.Setup(r => r.Save()).Returns(false);

        await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_WhenSaveFails()
    {
        var createDto = new IngredientCreateDto { Name = "Flour", Unit = "grams" };
        var entity = new IngredientEntity { Name = "Flour" };
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Add(entity));
        _repoMock.Setup(r => r.Save()).Returns(false);

        await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_MapsCorrectly()
    {
        var createDto = new IngredientCreateDto { Name = "Butter", Unit = "grams", CaloriesPerUnit = 7 };
        var entity = new IngredientEntity { Id = 5, Name = "Butter", Unit = "grams", CaloriesPerUnit = 7 };
        var dto = new IngredientDto { Id = 5, Name = "Butter", Unit = "grams", CaloriesPerUnit = 7 };
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Add(entity));
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        await _service.CreateAsync(createDto);

        _mapperMock.Verify(m => m.Map<IngredientEntity>(createDto), Times.Once);
        _mapperMock.Verify(m => m.Map<IngredientDto>(entity), Times.Once);
    }

    // ─── UpdateAsync (3 tests) ────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_Success_ReturnsUpdatedDto()
    {
        var updateDto = new IngredientUpdateDto { Name = "Updated Salt", Unit = "kg" };
        var existingEntity = new IngredientEntity { Id = 4, Name = "Salt", Unit = "grams" };
        var updatedEntity = new IngredientEntity { Id = 4, Name = "Updated Salt", Unit = "kg" };
        var dto = new IngredientDto { Id = 4, Name = "Updated Salt", Unit = "kg" };

        _repoMock.Setup(r => r.GetSingle(4)).Returns(existingEntity);
        _mapperMock.Setup(m => m.Map(updateDto, existingEntity));
        _repoMock.Setup(r => r.Update(4, existingEntity)).Returns(updatedEntity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(updatedEntity)).Returns(dto);

        var result = await _service.UpdateAsync(4, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Updated Salt", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

        var result = await _service.UpdateAsync(99, new IngredientUpdateDto());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate()
    {
        var updateDto = new IngredientUpdateDto { Name = "New Name", Unit = "ml", CaloriesPerUnit = 10 };
        var existingEntity = new IngredientEntity { Id = 6, Name = "Old Name", Unit = "grams", CaloriesPerUnit = 5 };
        var dto = new IngredientDto { Id = 6, Name = "New Name", Unit = "ml", CaloriesPerUnit = 10 };

        _repoMock.Setup(r => r.GetSingle(6)).Returns(existingEntity);
        _mapperMock.Setup(m => m.Map(updateDto, existingEntity));
        _repoMock.Setup(r => r.Update(6, existingEntity)).Returns(existingEntity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(existingEntity)).Returns(dto);

        var result = await _service.UpdateAsync(6, updateDto);

        _mapperMock.Verify(m => m.Map(updateDto, existingEntity), Times.Once);
        _repoMock.Verify(r => r.Update(6, existingEntity), Times.Once);
    }

    // ─── DeleteAsync (2 tests) ────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_Success_ReturnsTrue()
    {
        var entity = new IngredientEntity { Id = 7, Name = "Oil" };
        _repoMock.Setup(r => r.GetSingle(7)).Returns(entity);
        _repoMock.Setup(r => r.Delete(7));
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.DeleteAsync(7);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

        var result = await _service.DeleteAsync(99);

        Assert.False(result);
    }

    // ─── SearchAsync (2 tests) ────────────────────────────────────────────────

    [Fact]
    public async Task SearchAsync_ReturnsMatches()
    {
        var entities = new List<IngredientEntity>
        {
            new IngredientEntity { Id = 1, Name = "Salt", Unit = "grams" },
            new IngredientEntity { Id = 2, Name = "Sea Salt", Unit = "grams" }
        };
        var dtos = new List<IngredientDto>
        {
            new IngredientDto { Id = 1, Name = "Salt", Unit = "grams" },
            new IngredientDto { Id = 2, Name = "Sea Salt", Unit = "grams" }
        };
        _repoMock.Setup(r => r.SearchByName("salt")).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

        var result = await _service.SearchAsync("salt");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
    {
        var entities = new List<IngredientEntity>();
        var dtos = new List<IngredientDto>();
        _repoMock.Setup(r => r.SearchByName("xyz")).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

        var result = await _service.SearchAsync("xyz");

        Assert.Empty(result);
    }
}
