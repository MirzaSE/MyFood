using System.ComponentModel.DataAnnotations;

namespace MyFood.Domain.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Unit { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal CaloriesPerUnit { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Protein { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Carbs { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Fat { get; set; }

        public ICollection<FoodIngredientEntity> FoodIngredients { get; set; } = new List<FoodIngredientEntity>();
    }
}