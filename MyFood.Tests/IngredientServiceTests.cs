using AutoMapper;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;
using Xunit;

namespace MyFood.Tests;

public class IngredientServiceTests
{
    private readonly IngredientService _service;
    private readonly InMemoryIngredientRepository _repository;

    public IngredientServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<IngredientMappings>());
        var mapper = config.CreateMapper();
        _repository = new InMemoryIngredientRepository();
        _service = new IngredientService(_repository, mapper);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllIngredients()
    {
        _repository.Seed(new[]
        {
            new IngredientEntity { Id = 1, Name = "Salt", Unit = "tsp", CaloriesPerUnit = 0, Protein = 0, Carbs = 0, Fat = 0 },
            new IngredientEntity { Id = 2, Name = "Sugar", Unit = "g", CaloriesPerUnit = 4, Protein = 0, Carbs = 1, Fat = 0 }
        });

        var result = await _service.GetAllAsync(new QueryParameters { Page = 1, PageCount = 10 });

        Assert.Collection(result,
            item => Assert.Equal("Salt", item.Name),
            item => Assert.Equal("Sugar", item.Name));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyWhenNoIngredients()
    {
        var result = await _service.GetAllAsync(new QueryParameters { Page = 1, PageCount = 10 });
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_PaginationWorks()
    {
        _repository.Seed(Enumerable.Range(1, 5).Select(i => new IngredientEntity
        {
            Id = i,
            Name = $"Ingredient{i}",
            Unit = "unit",
            CaloriesPerUnit = i,
            Protein = i,
            Carbs = i,
            Fat = i
        }));

        var result = await _service.GetAllAsync(new QueryParameters { Page = 2, PageCount = 2 });

        Assert.Equal(2, result.Count());
        Assert.Equal("Ingredient3", result.ElementAt(0).Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsFoundIngredient()
    {
        _repository.Seed(new[] { new IngredientEntity { Id = 5, Name = "Pepper", Unit = "tsp", CaloriesPerUnit = 2, Protein = 0, Carbs = 0, Fat = 0 } });

        var result = await _service.GetByIdAsync(5);

        Assert.NotNull(result);
        Assert.Equal("Pepper", result!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullWhenNotFound()
    {
        var result = await _service.GetByIdAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_HandlesNegativeIds()
    {
        var result = await _service.GetByIdAsync(-1);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_SuccessfullyCreatesIngredient()
    {
        var dto = new IngredientCreateDto
        {
            Name = "Olive Oil",
            Unit = "tbsp",
            CaloriesPerUnit = 119,
            Protein = 0,
            Carbs = 0,
            Fat = 13.5
        };

        var created = await _service.CreateAsync(dto);

        Assert.Equal("Olive Oil", created.Name);
        Assert.Equal(119, created.CaloriesPerUnit);
        Assert.Equal(1, _repository.Count());
    }

    [Fact]
    public async Task CreateAsync_ThrowsWhenNameIsNullOrEmpty()
    {
        var dto = new IngredientCreateDto
        {
            Name = "",
            Unit = "g",
            CaloriesPerUnit = 1,
            Protein = 0,
            Carbs = 0,
            Fat = 0
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsOnDuplicateName()
    {
        _repository.Seed(new[] { new IngredientEntity { Id = 1, Name = "Salt", Unit = "tsp", CaloriesPerUnit = 0, Protein = 0, Carbs = 0, Fat = 0 } });

        var dto = new IngredientCreateDto
        {
            Name = "Salt",
            Unit = "tsp",
            CaloriesPerUnit = 0,
            Protein = 0,
            Carbs = 0,
            Fat = 0
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_MapsCorrectly()
    {
        var dto = new IngredientCreateDto
        {
            Name = "Tomato",
            Unit = "piece",
            CaloriesPerUnit = 18,
            Protein = 0.9,
            Carbs = 3.9,
            Fat = 0.2
        };

        var created = await _service.CreateAsync(dto);

        Assert.Equal("Tomato", created.Name);
        Assert.Equal("piece", created.Unit);
        Assert.Equal(0.9, created.Protein);
    }

    [Fact]
    public async Task UpdateAsync_SuccessfullyUpdatesIngredient()
    {
        _repository.Seed(new[] { new IngredientEntity { Id = 3, Name = "Sugar", Unit = "g", CaloriesPerUnit = 4, Protein = 0, Carbs = 1, Fat = 0 } });

        var dto = new IngredientUpdateDto { Unit = "tbsp", CaloriesPerUnit = 15 };
        var updated = await _service.UpdateAsync(3, dto);

        Assert.NotNull(updated);
        Assert.Equal("tbsp", updated!.Unit);
        Assert.Equal(15, updated.CaloriesPerUnit);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNullWhenNotFound()
    {
        var result = await _service.UpdateAsync(77, new IngredientUpdateDto { Unit = "g" });
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdateAppliesOnlyProvidedFields()
    {
        _repository.Seed(new[] { new IngredientEntity { Id = 4, Name = "Flour", Unit = "g", CaloriesPerUnit = 4, Protein = 0, Carbs = 1, Fat = 0 } });

        var dto = new IngredientUpdateDto { Protein = 0.2 };
        var updated = await _service.UpdateAsync(4, dto);

        Assert.NotNull(updated);
        Assert.Equal(0.2, updated!.Protein);
        Assert.Equal("g", updated.Unit);
    }

    [Fact]
    public async Task DeleteAsync_RemovesIngredient()
    {
        _repository.Seed(new[] { new IngredientEntity { Id = 6, Name = "Cinnamon", Unit = "tsp", CaloriesPerUnit = 6, Protein = 0, Carbs = 2, Fat = 0 } });

        var deleted = await _service.DeleteAsync(6);

        Assert.True(deleted);
        Assert.Empty(_repository.Items);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalseWhenNotFound()
    {
        var deleted = await _service.DeleteAsync(999);
        Assert.False(deleted);
    }

    [Fact]
    public async Task SearchAsync_FindsMatchesCaseInsensitive()
    {
        _repository.Seed(new[]
        {
            new IngredientEntity { Id = 1, Name = "Olive Oil", Unit = "tbsp", CaloriesPerUnit = 119, Protein = 0, Carbs = 0, Fat = 13.5 },
            new IngredientEntity { Id = 2, Name = "Oil", Unit = "ml", CaloriesPerUnit = 9, Protein = 0, Carbs = 0, Fat = 1 }
        });

        var result = await _service.SearchAsync("oil");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmptyWhenNoMatch()
    {
        _repository.Seed(new[] { new IngredientEntity { Id = 1, Name = "Salt", Unit = "tsp", CaloriesPerUnit = 0, Protein = 0, Carbs = 0, Fat = 0 } });

        var result = await _service.SearchAsync("pepper");

        Assert.Empty(result);
    }

    private class InMemoryIngredientRepository : IIngredientRepository
    {
        public List<IngredientEntity> Items { get; } = new List<IngredientEntity>();

        public void Seed(IEnumerable<IngredientEntity> ingredients)
        {
            Items.Clear();
            Items.AddRange(ingredients);
        }

        public IngredientEntity? GetSingle(int id) => Items.FirstOrDefault(x => x.Id == id);

        public void Add(IngredientEntity item)
        {
            item.Id = Items.Any() ? Items.Max(x => x.Id) + 1 : 1;
            Items.Add(item);
        }

        public IngredientEntity Update(IngredientEntity item)
        {
            var existing = GetSingle(item.Id);
            if (existing == null)
            {
                return item;
            }

            existing.Name = item.Name;
            existing.Unit = item.Unit;
            existing.CaloriesPerUnit = item.CaloriesPerUnit;
            existing.Protein = item.Protein;
            existing.Carbs = item.Carbs;
            existing.Fat = item.Fat;
            return existing;
        }

        public void Delete(IngredientEntity item)
        {
            Items.RemoveAll(x => x.Id == item.Id);
        }

        public IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters)
        {
            var query = Items.AsQueryable().OrderBy(x => x.Name);
            if (queryParameters.HasQuery())
            {
                var lower = queryParameters.Query!.ToLowerInvariant();
                query = query.Where(x => x.Name!.ToLower().Contains(lower) || x.Unit!.ToLower().Contains(lower));
            }

            return query.Skip(queryParameters.PageCount * (queryParameters.Page - 1)).Take(queryParameters.PageCount);
        }

        public bool ExistsByName(string name)
        {
            return Items.Any(x => x.Name != null && x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<IngredientEntity> SearchByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Enumerable.Empty<IngredientEntity>();
            }

            var lower = name.ToLowerInvariant();
            return Items.Where(x => x.Name!.ToLower().Contains(lower)).ToList();
        }

        public int Count() => Items.Count;

        public bool Save() => true;
    }
}
