using System.ComponentModel.DataAnnotations;

namespace MyFood.Domain.Entities
{
    public class FoodEntity
    {
        public int Id { get; set; }

        [MaxLength(150)]
        public string? Name { get; set; }

        [MaxLength(150)]
        public string? Type { get; set; }

        public int Calories { get; set; }

        public DateTime Created { get; set; }

        public ICollection<IngredientEntity> Ingredients { get; set; } = new List<IngredientEntity>();
    }
}