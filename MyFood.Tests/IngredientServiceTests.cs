using AutoMapper;
using Moq;
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

    [Fact]
    public async Task GetAllAsync_ReturnsAllIngredients()
    {
        var entities = new List<IngredientEntity>
        {
            new IngredientEntity { Id = 1, Name = "Salt", FoodId = 1 },
            new IngredientEntity { Id = 2, Name = "Pepper", FoodId = 1 }
        };
        var dtos = new List<IngredientDto>
        {
            new IngredientDto { Id = 1, Name = "Salt", FoodId = 1 },
            new IngredientDto { Id = 2, Name = "Pepper", FoodId = 1 }
        };

        _repoMock.Setup(r => r.GetAll()).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns(dtos);

        var result = await _service.GetAllAsync(new QueryParameters { Page = 1, PageCount = 10 });

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoIngredients()
    {
        _repoMock.Setup(r => r.GetAll()).Returns(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns(new List<IngredientDto>());

        var result = await _service.GetAllAsync(new QueryParameters { Page = 1, PageCount = 10 });

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_Pagination_Works()
    {
        var entities = Enumerable.Range(1, 10)
            .Select(i => new IngredientEntity
            {
                Id = i,
                Name = $"Item{i:D2}",
                FoodId = 1
            })
            .ToList();

        _repoMock.Setup(r => r.GetAll()).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns((IEnumerable<IngredientEntity> src) =>
                src.Select(e => new IngredientDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    FoodId = e.FoodId
                }).ToList());

        var result = await _service.GetAllAsync(new QueryParameters { Page = 2, PageCount = 3 });

        Assert.Equal(3, result.Count());
        Assert.Equal(4, result.First().Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsIngredient_WhenFound()
    {
        var entity = new IngredientEntity { Id = 5, Name = "Salt", FoodId = 1 };
        var dto = new IngredientDto { Id = 5, Name = "Salt", FoodId = 1 };

        _repoMock.Setup(r => r.GetSingle(5)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.GetByIdAsync(5);

        Assert.NotNull(result);
        Assert.Equal("Salt", result?.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).Returns((IngredientEntity?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_ForNegativeId()
    {
        var result = await _service.GetByIdAsync(-1);

        Assert.Null(result);
        _repoMock.Verify(r => r.GetSingle(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_Success_MapsCorrectly()
    {
        var createDto = new IngredientCreateDto { Name = "Sugar", FoodId = 2 };
        var entity = new IngredientEntity { Id = 7, Name = "Sugar", FoodId = 2 };
        var dto = new IngredientDto { Id = 7, Name = "Sugar", FoodId = 2 };

        _repoMock.Setup(r => r.GetAll()).Returns(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.CreateAsync(createDto);

        Assert.Equal("Sugar", result.Name);
        _repoMock.Verify(r => r.Add(entity), Times.Once);
        _mapperMock.Verify(m => m.Map<IngredientEntity>(createDto), Times.Once);
        _mapperMock.Verify(m => m.Map<IngredientDto>(entity), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenNameIsNull()
    {
        var createDto = new IngredientCreateDto { Name = null, FoodId = 1 };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenDuplicateExists()
    {
        var createDto = new IngredientCreateDto { Name = "Salt", FoodId = 1 };
        var existing = new List<IngredientEntity>
        {
            new IngredientEntity { Id = 1, Name = "salt", FoodId = 1 }
        };

        _repoMock.Setup(r => r.GetAll()).Returns(existing);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_Success()
    {
        var updateDto = new IngredientUpdateDto { Name = "Updated", FoodId = 3 };
        var entity = new IngredientEntity { Id = 10, Name = "Old", FoodId = 1 };
        var updatedEntity = new IngredientEntity { Id = 10, Name = "Updated", FoodId = 3 };
        var dto = new IngredientDto { Id = 10, Name = "Updated", FoodId = 3 };

        _repoMock.Setup(r => r.GetSingle(10)).Returns(entity);
        _repoMock.Setup(r => r.Update(entity)).Returns(updatedEntity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(updatedEntity)).Returns(dto);

        var result = await _service.UpdateAsync(10, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Updated", result?.Name);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(404)).Returns((IngredientEntity?)null);

        var result = await _service.UpdateAsync(404, new IngredientUpdateDto { Name = "Test" });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_AllowsPartialUpdate()
    {
        var updateDto = new IngredientUpdateDto { FoodId = 5 };
        var entity = new IngredientEntity { Id = 12, Name = "Salt", FoodId = 1 };
        IngredientEntity? captured = null;

        _repoMock.Setup(r => r.GetSingle(12)).Returns(entity);
        _repoMock.Setup(r => r.Update(It.IsAny<IngredientEntity>()))
            .Callback<IngredientEntity>(e => captured = e)
            .Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(It.IsAny<IngredientEntity>()))
            .Returns(new IngredientDto { Id = 12, Name = "Salt", FoodId = 5 });

        var result = await _service.UpdateAsync(12, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Salt", captured?.Name);
        Assert.Equal(5, captured?.FoodId);
    }

    [Fact]
    public async Task DeleteAsync_Success()
    {
        var entity = new IngredientEntity { Id = 3, Name = "Salt", FoodId = 1 };

        _repoMock.Setup(r => r.GetSingle(3)).Returns(entity);
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.DeleteAsync(3);

        Assert.True(result);
        _repoMock.Verify(r => r.Delete(3), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(77)).Returns((IngredientEntity?)null);

        var result = await _service.DeleteAsync(77);

        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_FindsMatches()
    {
        var entities = new List<IngredientEntity>
        {
            new IngredientEntity { Id = 1, Name = "Salt", FoodId = 1 },
            new IngredientEntity { Id = 2, Name = "Pepper", FoodId = 1 }
        };

        _repoMock.Setup(r => r.GetAll()).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns((IEnumerable<IngredientEntity> src) =>
                src.Select(e => new IngredientDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    FoodId = e.FoodId
                }).ToList());

        var result = await _service.SearchAsync("pep");

        Assert.Single(result);
        Assert.Equal("Pepper", result.First().Name);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
    {
        var entities = new List<IngredientEntity>
        {
            new IngredientEntity { Id = 1, Name = "Salt", FoodId = 1 }
        };

        _repoMock.Setup(r => r.GetAll()).Returns(entities);
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
            new IngredientEntity { Id = 1, Name = "Salt", FoodId = 1 }
        };

        _repoMock.Setup(r => r.GetAll()).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns((IEnumerable<IngredientEntity> src) =>
                src.Select(e => new IngredientDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    FoodId = e.FoodId
                }).ToList());

        var result = await _service.SearchAsync("sAlT");

        Assert.Single(result);
    }
}
