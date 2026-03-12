using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Entities
{
    public class FoodEntity
    {
        public int Id { get; set; }
        [MaxLength(260)]
        public string? Name { get; set; }
        [MaxLength(260)]
        public string? Type { get; set; }
        public int Calories { get; set; }
        public DateTime Created { get; set; }

        public ICollection<IngredientEntity> Ingredients { get; set; } = new List<IngredientEntity>();
    }
}
