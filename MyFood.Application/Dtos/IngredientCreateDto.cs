using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Unit { get; set; } = null!;

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

        [Required]
        public int FoodId { get; set; }
    }
}
