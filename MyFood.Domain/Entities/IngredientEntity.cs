using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Domain.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        [Required]
        public string Unit { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public double CaloriesPerUnit { get; set; }

        [Range(0, double.MaxValue)]
        public double Protein { get; set; }

        [Range(0, double.MaxValue)]
        public double Carbs { get; set; }

        [Range(0, double.MaxValue)]
        public double Fat { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        public int FoodId { get; set; }

        [ForeignKey("FoodId")]
        public FoodEntity FoodItem { get; set; } = null!;
    }
}