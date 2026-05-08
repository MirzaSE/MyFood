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
        var entities = new List<IngredientEntity> { new IngredientEntity { Id = 1, Name = "Sugar" } };
        var dtos = new List<IngredientDto> { new IngredientDto { Id = 1, Name = "Sugar" } };

        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

        var result = await _service.GetAllAsync(new QueryParameters());

        Assert.Single(result);
        Assert.Equal("Sugar", result.First().Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoIngredients()
    {
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns(new List<IngredientDto>());

        var result = await _service.GetAllAsync(new QueryParameters());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_UsesPaginationParameters()
    {
        var query = new QueryParameters { Page = 2, PageCount = 5 };
        _repoMock.Setup(r => r.GetAll(It.Is<QueryParameters>(q => q.Page == 2 && q.PageCount == 5)))
            .Returns(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns(new List<IngredientDto>());

        await _service.GetAllAsync(query);

        _repoMock.Verify(r => r.GetAll(It.Is<QueryParameters>(q => q.Page == 2 && q.PageCount == 5)), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsIngredient_WhenFound()
    {
        var entity = new IngredientEntity { Id = 2, Name = "Salt" };
        var dto = new IngredientDto { Id = 2, Name = "Salt" };

        _repoMock.Setup(r => r.GetSingle(2)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.GetByIdAsync(2);

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
    public async Task GetByIdAsync_Throws_WhenNegativeId()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.GetByIdAsync(-1));
    }

    [Fact]
    public async Task CreateAsync_Succeeds_WhenValid()
    {
        var createDto = new IngredientCreateDto { Name = "Flour", Unit = "g", CaloriesPerUnit = 3 };
        var entity = new IngredientEntity { Id = 5, Name = "Flour", Unit = "g", CaloriesPerUnit = 3 };
        var dto = new IngredientDto { Id = 5, Name = "Flour", Unit = "g", CaloriesPerUnit = 3 };

        _repoMock.Setup(r => r.SearchByName("Flour")).Returns(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Add(entity));
        _repoMock.Setup(r => r.Save()).Returns(true);
        _repoMock.Setup(r => r.GetSingle(entity.Id)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.CreateAsync(createDto);

        Assert.Equal("Flour", result.Name);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenNameIsMissing()
    {
        var createDto = new IngredientCreateDto { Name = " ", Unit = "g" };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenDuplicateName()
    {
        var createDto = new IngredientCreateDto { Name = "Butter", Unit = "g" };
        var existing = new IngredientEntity { Id = 1, Name = "Butter" };

        _repoMock.Setup(r => r.SearchByName("Butter")).Returns(new List<IngredientEntity> { existing });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_MapsToEntityAndDto()
    {
        var createDto = new IngredientCreateDto { Name = "Milk", Unit = "ml" };
        var entity = new IngredientEntity { Id = 6, Name = "Milk", Unit = "ml" };
        var dto = new IngredientDto { Id = 6, Name = "Milk", Unit = "ml" };

        _repoMock.Setup(r => r.SearchByName("Milk")).Returns(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IngredientEntity>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.Add(entity));
        _repoMock.Setup(r => r.Save()).Returns(true);
        _repoMock.Setup(r => r.GetSingle(entity.Id)).Returns(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.CreateAsync(createDto);

        _mapperMock.Verify(m => m.Map<IngredientEntity>(createDto), Times.Once);
        _mapperMock.Verify(m => m.Map<IngredientDto>(entity), Times.Once);
        Assert.Equal("Milk", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_Succeeds_WhenFound()
    {
        var updateDto = new IngredientUpdateDto { Name = "Cheese", Unit = "g", CaloriesPerUnit = 4 };
        var existing = new IngredientEntity { Id = 7, Name = "Old", Unit = "g" };
        var updated = new IngredientEntity { Id = 7, Name = "Cheese", Unit = "g", CaloriesPerUnit = 4 };
        var dto = new IngredientDto { Id = 7, Name = "Cheese", Unit = "g", CaloriesPerUnit = 4 };

        _repoMock.Setup(r => r.GetSingle(7)).Returns(existing);
        _repoMock.Setup(r => r.Update(7, existing)).Returns(updated);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(updated)).Returns(dto);

        var result = await _service.UpdateAsync(7, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Cheese", result?.Name);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(88)).Returns((IngredientEntity?)null);

        var result = await _service.UpdateAsync(88, new IngredientUpdateDto());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate_KeepsExistingNameAndUnit()
    {
        var updateDto = new IngredientUpdateDto { Name = null, Unit = null, CaloriesPerUnit = 2 };
        var existing = new IngredientEntity { Id = 9, Name = "Honey", Unit = "g" };
        var updated = new IngredientEntity { Id = 9, Name = "Honey", Unit = "g", CaloriesPerUnit = 2 };
        var dto = new IngredientDto { Id = 9, Name = "Honey", Unit = "g", CaloriesPerUnit = 2 };

        _repoMock.Setup(r => r.GetSingle(9)).Returns(existing);
        _repoMock.Setup(r => r.Update(9, existing)).Returns(updated);
        _repoMock.Setup(r => r.Save()).Returns(true);
        _mapperMock.Setup(m => m.Map<IngredientDto>(updated)).Returns(dto);

        var result = await _service.UpdateAsync(9, updateDto);

        Assert.Equal("Honey", result?.Name);
        Assert.Equal("g", result?.Unit);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenDeleted()
    {
        var existing = new IngredientEntity { Id = 10, Name = "Oil" };

        _repoMock.Setup(r => r.GetSingle(10)).Returns(existing);
        _repoMock.Setup(r => r.Delete(10));
        _repoMock.Setup(r => r.Save()).Returns(true);

        var result = await _service.DeleteAsync(10);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(123)).Returns((IngredientEntity?)null);

        var result = await _service.DeleteAsync(123);

        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatches_CaseInsensitive()
    {
        var entities = new List<IngredientEntity> { new IngredientEntity { Id = 11, Name = "Sugar" } };
        var dtos = new List<IngredientDto> { new IngredientDto { Id = 11, Name = "Sugar" } };

        _repoMock.Setup(r => r.SearchByName(It.Is<string>(s => s.Equals("sugar", StringComparison.OrdinalIgnoreCase))))
            .Returns(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

        var result = await _service.SearchAsync("SUGAR");

        Assert.Single(result);
        Assert.Equal("Sugar", result.First().Name);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
    {
        _repoMock.Setup(r => r.SearchByName(It.IsAny<string>())).Returns(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns(new List<IngredientDto>());

        var result = await _service.SearchAsync("DoesNotExist");

        Assert.Empty(result);
    }
}
