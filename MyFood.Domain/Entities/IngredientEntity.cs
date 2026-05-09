using System.ComponentModel.DataAnnotations;

namespace MyFood.Domain.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Unit { get; set; } = string.Empty;

        public double CaloriesPerUnit { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }

        public List<FoodIngredientEntity> FoodIngredients { get; set; } = new();
    }
}
