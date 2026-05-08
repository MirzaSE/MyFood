using Xunit;
using Moq;
using AutoMapper;
using FluentAssertions;
using MyFood.Application;
using MyFood.Application.Services;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

public class IngredientServiceTests
{
    private readonly Mock<IIngredientRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IngredientService _service;

    public IngredientServiceTests()
    {
        _repoMock = new Mock<IIngredientRepository>();
        _mapperMock = new Mock<IMapper>();

        _service = new IngredientService(
            _repoMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task GetAllIngredientsAsync_ReturnsIngredients()
    {
        var ingredients = new List<IngredientEntity>
        {
            new IngredientEntity
            {
                Id = 1,
                Name = "Rice"
            }
        }.AsQueryable();

        var mapped = new List<IngredientDto>
        {
            new IngredientDto
            {
                Id = 1,
                Name = "Rice"
            }
        };

        _repoMock.Setup(x =>
            x.GetAll(It.IsAny<QueryParameters>()))
            .Returns(ingredients);

        _mapperMock.Setup(x =>
            x.Map<IEnumerable<IngredientDto>>(It.IsAny<object>()))
            .Returns(mapped);

        var result = await _service
            .GetAllIngredientsAsync(new QueryParameters());

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetIngredientByIdAsync_ReturnsIngredient()
    {
        var ingredient = new IngredientEntity
        {
            Id = 1,
            Name = "Rice"
        };

        var dto = new IngredientDto
        {
            Id = 1,
            Name = "Rice"
        };

        _repoMock.Setup(x => x.GetSingle(1))
            .Returns(ingredient);

        _mapperMock.Setup(x =>
            x.Map<IngredientDto>(ingredient))
            .Returns(dto);

        var result = await _service
            .GetIngredientByIdAsync(1);

        result.Should().NotBeNull();
        result.Name.Should().Be("Rice");
    }

    [Fact]
    public async Task GetIngredientByIdAsync_ReturnsNull()
    {
        _repoMock.Setup(x => x.GetSingle(99))
            .Returns((IngredientEntity)null);

        var result = await _service
            .GetIngredientByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateIngredientAsync_CreatesIngredient()
    {
        var createDto = new IngredientCreateDto
        {
            Name = "Rice"
        };

        var entity = new IngredientEntity
        {
            Name = "Rice"
        };

        var dto = new IngredientDto
        {
            Name = "Rice"
        };

        _mapperMock.Setup(x =>
            x.Map<IngredientEntity>(createDto))
            .Returns(entity);

        _mapperMock.Setup(x =>
            x.Map<IngredientDto>(entity))
            .Returns(dto);

        var result = await _service
            .CreateIngredientAsync(createDto);

        result.Should().NotBeNull();

        _repoMock.Verify(x =>
            x.Add(It.IsAny<IngredientEntity>()),
            Times.Once);

        _repoMock.Verify(x =>
            x.Save(),
            Times.Once);
    }

    [Fact]
    public async Task UpdateIngredientAsync_UpdatesIngredient()
    {
        var existing = new IngredientEntity
        {
            Id = 1,
            Name = "Rice"
        };

        var updateDto = new IngredientUpdateDto
        {
            Name = "Brown Rice"
        };

        var updated = new IngredientEntity
        {
            Id = 1,
            Name = "Brown Rice"
        };

        var mappedDto = new IngredientDto
        {
            Id = 1,
            Name = "Brown Rice"
        };

        _repoMock.Setup(x => x.GetSingle(1))
            .Returns(existing);

        _repoMock.Setup(x =>
            x.Update(1, existing))
            .Returns(updated);

        _mapperMock.Setup(x =>
            x.Map<IngredientDto>(updated))
            .Returns(mappedDto);

        var result = await _service
            .UpdateIngredientAsync(1, updateDto);

        result.Should().NotBeNull();
        result.Name.Should().Be("Brown Rice");
    }

    [Fact]
    public async Task DeleteIngredientAsync_ReturnsTrue()
    {
        var ingredient = new IngredientEntity
        {
            Id = 1,
            Name = "Rice"
        };

        _repoMock.Setup(x => x.GetSingle(1))
            .Returns(ingredient);

        _repoMock.Setup(x => x.Save())
            .Returns(true);

        var result = await _service
            .DeleteIngredientAsync(1);

        result.Should().BeTrue();
    }
}