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

    public IngredientServiceTests() => _service = new IngredientService(_repoMock.Object, _mapperMock.Object);

    [Fact] public async Task GetAllAsync_ReturnsAll() { var entities = new List<IngredientEntity> { new() { Id = 1, Name = "Rice", Unit = "g" } }.AsQueryable(); _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(entities); _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto> { new() { Id = 1, Name = "Rice" } }); Assert.Single(await _service.GetAllAsync(new QueryParameters())); }
    [Fact] public async Task GetAllAsync_ReturnsEmpty() { _repoMock.Setup(r => r.GetAll(It.IsAny<QueryParameters>())).Returns(new List<IngredientEntity>().AsQueryable()); _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto>()); Assert.Empty(await _service.GetAllAsync(new QueryParameters())); }
    [Fact] public async Task GetByIdAsync_Found() { var entity = new IngredientEntity { Id = 1, Name = "Rice", Unit = "g" }; _repoMock.Setup(r => r.GetById(1)).Returns(entity); _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(new IngredientDto { Id = 1, Name = "Rice" }); Assert.NotNull(await _service.GetByIdAsync(1)); }
    [Fact] public async Task GetByIdAsync_NotFound() { _repoMock.Setup(r => r.GetById(99)).Returns((IngredientEntity?)null); Assert.Null(await _service.GetByIdAsync(99)); }
    [Fact] public async Task GetByIdAsync_Negative() { Assert.Null(await _service.GetByIdAsync(-1)); }
    [Fact] public async Task CreateAsync_Success() { var dto = new IngredientCreateDto { Name = "Rice", Unit = "g" }; var entity = new IngredientEntity { Id = 1, Name = "Rice", Unit = "g" }; _repoMock.Setup(r => r.Search("Rice")).Returns(new List<IngredientEntity>()); _mapperMock.Setup(m => m.Map<IngredientEntity>(dto)).Returns(entity); _repoMock.Setup(r => r.Save()).Returns(true); _mapperMock.Setup(m => m.Map<IngredientDto>(entity)).Returns(new IngredientDto { Id = 1, Name = "Rice" }); var result = await _service.CreateAsync(dto); Assert.Equal("Rice", result.Name); }
    [Fact] public async Task CreateAsync_NullNameError() { await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(new IngredientCreateDto { Name = "" })); }
    [Fact] public async Task CreateAsync_DuplicateError() { _repoMock.Setup(r => r.Search("Rice")).Returns(new List<IngredientEntity> { new() { Name = "Rice", Unit = "g" } }); await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(new IngredientCreateDto { Name = "Rice", Unit = "g" })); }
    [Fact] public async Task UpdateAsync_Success() { var existing = new IngredientEntity { Id = 1, Name = "Old", Unit = "g" }; _repoMock.Setup(r => r.GetById(1)).Returns(existing); _repoMock.Setup(r => r.Save()).Returns(true); _mapperMock.Setup(m => m.Map(It.IsAny<IngredientUpdateDto>(), existing)); _mapperMock.Setup(m => m.Map<IngredientDto>(existing)).Returns(new IngredientDto { Id = 1, Name = "New" }); var result = await _service.UpdateAsync(1, new IngredientUpdateDto { Name = "New", Unit = "g" }); Assert.NotNull(result); }
    [Fact] public async Task UpdateAsync_NotFound() { _repoMock.Setup(r => r.GetById(1)).Returns((IngredientEntity?)null); Assert.Null(await _service.UpdateAsync(1, new IngredientUpdateDto { Name = "a", Unit = "g" })); }
    [Fact] public async Task DeleteAsync_Success() { _repoMock.Setup(r => r.GetById(1)).Returns(new IngredientEntity { Id = 1, Name = "Rice", Unit = "g" }); _repoMock.Setup(r => r.Save()).Returns(true); Assert.True(await _service.DeleteAsync(1)); }
    [Fact] public async Task DeleteAsync_NotFound() { _repoMock.Setup(r => r.GetById(1)).Returns((IngredientEntity?)null); Assert.False(await _service.DeleteAsync(1)); }
    [Fact] public async Task SearchAsync_FindsMatches() { _repoMock.Setup(r => r.Search("ri")).Returns(new List<IngredientEntity> { new() { Name = "Rice", Unit = "g" } }); _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto> { new() { Name = "Rice" } }); Assert.Single(await _service.SearchAsync("ri")); }
    [Fact] public async Task SearchAsync_ReturnsEmpty() { _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto>()); Assert.Empty(await _service.SearchAsync("")); }
    [Fact] public async Task SearchAsync_CaseInsensitive() { _repoMock.Setup(r => r.Search("RICE")).Returns(new List<IngredientEntity> { new() { Name = "Rice", Unit = "g" } }); _mapperMock.Setup(m => m.Map<IEnumerable<IngredientDto>>(It.IsAny<IEnumerable<IngredientEntity>>())).Returns(new List<IngredientDto> { new() { Name = "Rice" } }); Assert.Single(await _service.SearchAsync("RICE")); }
}
