using System.ComponentModel.DataAnnotations;

namespace MyFood.Domain.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(260)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Unit { get; set; } = string.Empty;

        [Range(0.01, 100000)]
        public decimal CaloriesPerUnit { get; set; }

        [Range(0, 100000)]
        public decimal Protein { get; set; }

        [Range(0, 100000)]
        public decimal Carbs { get; set; }

        [Range(0, 100000)]
        public decimal Fat { get; set; }

        public ICollection<FoodIngredientEntity> FoodIngredients { get; set; } = new List<FoodIngredientEntity>();
    }
}
