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

    // ─── GetAllAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsAllMappedDtos()
    {
        var entities = new List<IngredientEntity> { new() { Id = 1, Name = "Salt" } };
        var dtos = new List<IngredientDto> { new() { Id = 1, Name = "Salt" } };
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(entities)).Returns(dtos);

        var result = await _service.GetAllAsync(new QueryParameters());

        Assert.Single(result);
        Assert.Equal("Salt", result.First().Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoIngredients()
    {
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).ReturnsAsync(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns(new List<IngredientDto>());

        var result = await _service.GetAllAsync(new QueryParameters());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_PassesPaginationParametersToRepository()
    {
        var parameters = new QueryParameters { Page = 2, PageCount = 5 };
        _repoMock.Setup(r => r.GetAll(parameters)).ReturnsAsync(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns(new List<IngredientDto>());

        await _service.GetAllAsync(parameters);

        _repoMock.Verify(r => r.GetAll(parameters), Times.Once);
    }

    // ─── GetByIdAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenFound()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Pepper" };
        var dto = new IngredientDto { Id = 1, Name = "Pepper" };
        _repoMock.Setup(r => r.GetSingle(1)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(dto);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Pepper", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).ReturnsAsync((IngredientEntity?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsArgumentOutOfRange_WhenNegativeId()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.GetByIdAsync(-1));
    }

    // ─── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ReturnsDto_OnSuccess()
    {
        var dto = new IngredientCreateDto { Name = "Garlic" };
        var entity = new IngredientEntity { Id = 1, Name = "Garlic" };
        var resultDto = new IngredientDto { Id = 1, Name = "Garlic" };

        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).ReturnsAsync(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IngredientEntity>(dto)).Returns(entity);
        _repoMock.Setup(r => r.Add(entity)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(resultDto);

        var result = await _service.CreateAsync(dto);

        Assert.Equal("Garlic", result.Name);
    }

    [Fact]
    public async Task CreateAsync_ThrowsArgumentException_WhenNameIsNull()
    {
        var dto = new IngredientCreateDto { Name = null };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsInvalidOperationException_WhenDuplicate()
    {
        var dto = new IngredientCreateDto { Name = "Chicken" };
        var existing = new List<IngredientEntity> { new() { Name = "Chicken" } };
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).ReturnsAsync(existing);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_MapsCorrectly()
    {
        var dto = new IngredientCreateDto { Name = "Onion", Quantity = "2 pieces" };
        var entity = new IngredientEntity { Id = 2, Name = "Onion" };
        var resultDto = new IngredientDto { Id = 2, Name = "Onion", Quantity = "2 pieces" };

        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).ReturnsAsync(new List<IngredientEntity>());
        _mapperMock.Setup(m => m.Map<IngredientEntity>(dto)).Returns(entity);
        _repoMock.Setup(r => r.Add(entity)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(resultDto);

        var result = await _service.CreateAsync(dto);

        Assert.Equal("Onion", result.Name);
        Assert.Equal("2 pieces", result.Quantity);
        _mapperMock.Verify(m => m.Map<IngredientEntity>(dto), Times.Once);
    }

    // ─── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ReturnsUpdatedDto_OnSuccess()
    {
        var existing = new IngredientEntity { Id = 1, Name = "Old" };
        var updated = new IngredientEntity { Id = 1, Name = "New" };
        var dto = new IngredientUpdateDto { Name = "New" };
        var resultDto = new IngredientDto { Id = 1, Name = "New" };

        _repoMock.Setup(r => r.GetSingle(1)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.Update(1, existing)).ReturnsAsync(updated);
        _mapperMock.Setup(m => m.Map<IngredientDto>(updated)).Returns(resultDto);

        var result = await _service.UpdateAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("New", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSingle(99)).ReturnsAsync((IngredientEntity?)null);

        var result = await _service.UpdateAsync(99, new IngredientUpdateDto { Name = "X" });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate_OnlyChangesProvidedFields()
    {
        var existing = new IngredientEntity { Id = 1, Name = "Salt" };
        var dto = new IngredientUpdateDto { Name = "Sea Salt" };
        _repoMock.Setup(r => r.GetSingle(1)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.Update(1, existing)).ReturnsAsync(existing);
        _mapperMock.Setup(m => m.Map<IngredientDto>(existing)).Returns(new IngredientDto { Id = 1, Name = "Sea Salt" });

        var result = await _service.UpdateAsync(1, dto);

        Assert.Equal("Sea Salt", existing.Name);
        Assert.Equal("Sea Salt", result!.Name);
    }

    // ─── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_OnSuccess()
    {
        var entity = new IngredientEntity { Id = 1, Name = "Basil" };
        _repoMock.Setup(r => r.Delete(1)).ReturnsAsync(entity);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        _repoMock.Setup(r => r.Delete(99)).ReturnsAsync((IngredientEntity?)null);

        var result = await _service.DeleteAsync(99);

        Assert.False(result);
    }

    // ─── SearchAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task SearchAsync_ReturnsMatches()
    {
        var all = new List<IngredientEntity>
        {
            new() { Name = "Chicken Breast" },
            new() { Name = "Beef" }
        };
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).ReturnsAsync(all);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns((IEnumerable<IngredientEntity> src) => src.Select(e => new IngredientDto { Name = e.Name }));

        var result = await _service.SearchAsync("chicken");

        Assert.Single(result);
        Assert.Equal("Chicken Breast", result.First().Name);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
    {
        var all = new List<IngredientEntity> { new() { Name = "Salt" } };
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).ReturnsAsync(all);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns((IEnumerable<IngredientEntity> src) => src.Select(e => new IngredientDto { Name = e.Name }));

        var result = await _service.SearchAsync("xyz");

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_IsCaseInsensitive()
    {
        var all = new List<IngredientEntity> { new() { Name = "Oregano" } };
        _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).ReturnsAsync(all);
        _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>()))
            .Returns((IEnumerable<IngredientEntity> src) => src.Select(e => new IngredientDto { Name = e.Name }));

        var result = await _service.SearchAsync("OREGANO");

        Assert.Single(result);
        Assert.Equal("Oregano", result.First().Name);
    }
}
