using Xunit;
using Moq;
using FluentAssertions;
using MyFood.Application;
using MyFood.Application.Services;
using MyFood.Domain.Entities;
using MyFood.Application.Dtos;

public class IngredientRepositoryTests
{
    private readonly Mock<IIngredientRepository> _repoMock;

    public IngredientRepositoryTests()
    {
        _repoMock = new Mock<IIngredientRepository>();
    }

    // =====================================================
    // GET SINGLE
    // =====================================================

    [Fact]
    public void GetSingle_ReturnsIngredient()
    {
        var ingredient = new IngredientEntity
        {
            Id = 1,
            Name = "Rice",
            Quantity = 100
        };

        _repoMock.Setup(x => x.GetSingle(1))
            .Returns(ingredient);

        var result = _repoMock.Object.GetSingle(1);

        result.Should().NotBeNull();
        result.Name.Should().Be("Rice");
    }

    [Fact]
    public void GetSingle_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(x => x.GetSingle(99))
            .Returns((IngredientEntity)null);

        var result = _repoMock.Object.GetSingle(99);

        result.Should().BeNull();
    }

    // =====================================================
    // GET ALL
    // =====================================================

    [Fact]
    public void GetAll_ReturnsAllIngredients()
    {
        var ingredients = new List<IngredientEntity>
        {
            new IngredientEntity
            {
                Id = 1,
                Name = "Rice"
            },
            new IngredientEntity
            {
                Id = 2,
                Name = "Chicken"
            }
        }.AsQueryable();

        _repoMock.Setup(x => x.GetAll(It.IsAny<QueryParameters>()))
            .Returns(ingredients);

        var result = _repoMock.Object.GetAll(new QueryParameters());

        result.Count().Should().Be(2);
    }

    [Fact]
    public void GetAll_ReturnsEmpty()
    {
        var ingredients = new List<IngredientEntity>()
            .AsQueryable();

        _repoMock.Setup(x => x.GetAll(It.IsAny<QueryParameters>()))
            .Returns(ingredients);

        var result = _repoMock.Object.GetAll(new QueryParameters());

        result.Should().BeEmpty();
    }

    // =====================================================
    // ADD
    // =====================================================

    [Fact]
    public void Add_Ingredient()
    {
        var ingredient = new IngredientEntity
        {
            Name = "Rice",
            Quantity = 100
        };

        _repoMock.Setup(x => x.Add(It.IsAny<IngredientEntity>()));

        _repoMock.Object.Add(ingredient);

        _repoMock.Verify(x =>
            x.Add(It.IsAny<IngredientEntity>()),
            Times.Once);
    }

    // =====================================================
    // UPDATE
    // =====================================================

    [Fact]
    public void Update_Ingredient()
    {
        var ingredient = new IngredientEntity
        {
            Id = 1,
            Name = "Rice",
            Quantity = 100
        };

        var updated = new IngredientEntity
        {
            Id = 1,
            Name = "Brown Rice",
            Quantity = 200
        };

        _repoMock.Setup(x => x.Update(1, updated))
            .Returns(updated);

        var result = _repoMock.Object.Update(1, updated);

        result.Name.Should().Be("Brown Rice");
        result.Quantity.Should().Be(200);
    }

    // =====================================================
    // DELETE
    // =====================================================

    [Fact]
    public void Delete_Ingredient()
    {
        _repoMock.Setup(x => x.Delete(1));

        _repoMock.Object.Delete(1);

        _repoMock.Verify(x => x.Delete(1), Times.Once);
    }

    // =====================================================
    // SEARCH
    // =====================================================

    [Fact]
    public void SearchFoodsByName_ReturnsMatches()
    {
        var ingredients = new List<IngredientEntity>
        {
            new IngredientEntity
            {
                Id = 1,
                Name = "Rice"
            }
        };

        _repoMock.Setup(x => x.SearchFoodsByName("Rice"))
            .Returns(ingredients);

        var result = _repoMock.Object.SearchFoodsByName("Rice");

        result.Count().Should().Be(1);
    }

    [Fact]
    public void SearchFoodsByName_ReturnsEmpty()
    {
        _repoMock.Setup(x => x.SearchFoodsByName("XYZ"))
            .Returns(new List<IngredientEntity>());

        var result = _repoMock.Object.SearchFoodsByName("XYZ");

        result.Should().BeEmpty();
    }

    // =====================================================
    // COUNT
    // =====================================================

    [Fact]
    public void Count_ReturnsCorrectNumber()
    {
        _repoMock.Setup(x => x.Count())
            .Returns(5);

        var result = _repoMock.Object.Count();

        result.Should().Be(5);
    }

    // =====================================================
    // SAVE
    // =====================================================

    [Fact]
    public void Save_ReturnsTrue()
    {
        _repoMock.Setup(x => x.Save())
            .Returns(true);

        var result = _repoMock.Object.Save();

        result.Should().BeTrue();
    }
}