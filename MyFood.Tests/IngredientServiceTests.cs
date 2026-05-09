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

    // ─── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsAllItems()
    {
        var entities = new List<IngredientEntity>
        {
            new IngredientEntity { Id = 1, Name = "Salt" },
            new IngredientEntity { Id = 2, Name = "Pepper" }
        };
        var dtos = new List<IngredientDto>
        {
            new IngredientDto { Id = 1, Name = "Salt" },
            new IngredientDto { Id = 2, Name = "Pepper" }
        };
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.GetAllAsync(new QueryParameters());

        Assert.Equal(2, result.Count());
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
        var entities = new List<IngredientEntity> { new IngredientEntity { Id = 3, Name = "Sugar" } };
        var dtos = new List<IngredientDto> { new IngredientDto { Id = 3, Name = "Sugar" } };
        var queryParams = new QueryParameters { Page = 2, PageCount = 5 };
        _repoMock.Setup(r => r.GetAll(queryParams)).Returns(entities.AsQueryable());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(dtos);

        var result = await _service.GetAllAsync(queryParams);

        Assert.Single(result);
        Assert.Equal("Sugar", result.First().Name);
    }

    // ─── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenFound()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Salt", Unit = "g", CaloriesPerUnit = 0 };
        var dto = new IngredientDto { Id = 1, Name = "Salt", Unit = "g" };
        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Salt", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(999)).Returns((IngredientEntity?)null);

        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_ForNegativeId()
    {
        _repoMock.Setup(r => r.GetSingle(-1)).Returns((IngredientEntity?)null);

        var result = await _service.GetByIdAsync(-1);

        Assert.Null(result);
    }

    // ─── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_CreatesAndReturnsDto()
    {
        var createDto = new IngredientCreateDto { Name = "Flour", Unit = "g", CaloriesPerUnit = 3.5 };
        var entity = new IngredientEntity { Id = 10, Name = "Flour", Unit = "g", CaloriesPerUnit = 3.5 };
        var dto = new IngredientDto { Id = 10, Name = "Flour", Unit = "g", CaloriesPerUnit = 3.5 };

        _repoMock.Setup(r => r.SearchByName("Flour")).Returns(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Add(entity));
        _repoMock.Setup(r => r.Save()).Returns(true);
        _repoMock.Setup(r => r.GetSingle(entity.Id)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.CreateAsync(createDto);

        Assert.Equal("Flour", result.Name);
        Assert.Equal(3.5, result.CaloriesPerUnit);
    }

    [Fact]
    public async Task CreateAsync_ThrowsArgumentException_WhenNameIsEmpty()
    {
        var createDto = new IngredientCreateDto { Name = "   ", Unit = "g", CaloriesPerUnit = 1.0 };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsInvalidOperationException_WhenDuplicateName()
    {
        var createDto = new IngredientCreateDto { Name = "Salt", Unit = "g", CaloriesPerUnit = 0.1 };
        var existing = new IngredientEntity { Id = 1, Name = "Salt" };

        _repoMock.Setup(r => r.SearchByName("Salt")).Returns(new List<IngredientEntity> { existing });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_MapsCreateDtoToEntity()
    {
        var createDto = new IngredientCreateDto
        {
            Name = "Olive Oil",
            Unit = "ml",
            CaloriesPerUnit = 8.8,
            Protein = 0,
            Carbs = 0,
            Fat = 1.0
        };
        var entity = new IngredientEntity { Id = 5, Name = "Olive Oil", Unit = "ml", CaloriesPerUnit = 8.8, Fat = 1.0 };
        var dto = new IngredientDto { Id = 5, Name = "Olive Oil", Unit = "ml", CaloriesPerUnit = 8.8, Fat = 1.0 };

        _repoMock.Setup(r => r.SearchByName("Olive Oil")).Returns(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Add(entity));
        _repoMock.Setup(r => r.Save()).Returns(true);
        _repoMock.Setup(r => r.GetSingle(entity.Id)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.CreateAsync(createDto);

        _mapperMock.Verify(m => m.Map<IngredientEntity>(createDto), Times.Once);
        Assert.Equal("Olive Oil", result.Name);
        Assert.Equal(1.0, result.Fat);
    }

    // ─── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_UpdatesAndReturnsDto()
    {
        var updateDto = new IngredientUpdateDto { Name = "Sea Salt", Unit = "g" };
        var entity = new IngredientEntity { Id = 1, Name = "Salt", Unit = "g" };
        var dto = new IngredientDto { Id = 1, Name = "Sea Salt", Unit = "g" };

        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _mapperMock.Setup(m => m.Map(updateDto, entity));
        _repoMock.Setup(r => r.Update(1, entity)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.UpdateAsync(1, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Sea Salt", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(999)).Returns((IngredientEntity?)null);

        var result = await _service.UpdateAsync(999, new IngredientUpdateDto());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate_OnlyUpdatesProvidedFields()
    {
        var updateDto = new IngredientUpdateDto { CaloriesPerUnit = 5.0 };
        var entity = new IngredientEntity { Id = 2, Name = "Butter", Unit = "g", CaloriesPerUnit = 7.0 };
        var dto = new IngredientDto { Id = 2, Name = "Butter", Unit = "g", CaloriesPerUnit = 5.0 };

        _repoMock.Setup(r => r.GetSingle(2)).Returns(entity);
        _mapperMock.Setup(m => m.Map(updateDto, entity));
        _repoMock.Setup(r => r.Update(2, entity)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.UpdateAsync(2, updateDto);

        Assert.NotNull(result);
        _mapperMock.Verify(m => m.Map(updateDto, entity), Times.Once);
    }

    // ─── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_DeletesAndReturnsTrue()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Salt" };
        _repoMock.Setup(r => r.GetSingle(1)).Returns(entity);
        _repoMock.Setup(r => r.Delete(1));
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
        _repoMock.Verify(r => r.Delete(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(999)).Returns((IngredientEntity?)null);

        var result = await _service.DeleteAsync(999);

        Assert.False(result);
        _repoMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
    }

    // ─── SearchAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task SearchAsync_ReturnsMatchingIngredients()
    {
        var entities = new List<IngredientEntity>
        {
            new IngredientEntity { Id = 1, Name = "Sea Salt" },
            new IngredientEntity { Id = 2, Name = "Himalayan Salt" }
        };
        var dtos = new List<IngredientDto>
        {
            new IngredientDto { Id = 1, Name = "Sea Salt" },
            new IngredientDto { Id = 2, Name = "Himalayan Salt" }
        };
        _repoMock.Setup(r => r.SearchByName("salt")).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

        var result = await _service.SearchAsync("salt");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
    {
        _repoMock.Setup(r => r.SearchByName("xyz")).Returns(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns(new List<IngredientDto>());

        var result = await _service.SearchAsync("xyz");

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_IsCaseInsensitive_ViaRepository()
    {
        var entities = new List<IngredientEntity> { new IngredientEntity { Id = 1, Name = "Salt" } };
        var dtos = new List<IngredientDto> { new IngredientDto { Id = 1, Name = "Salt" } };

        _repoMock.Setup(r => r.SearchByName("SALT")).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

        var result = await _service.SearchAsync("SALT");

        Assert.Single(result);
        _repoMock.Verify(r => r.SearchByName("SALT"), Times.Once);
    }
}
