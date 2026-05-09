using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Unit is required")]
        [MaxLength(50)]
        public string Unit { get; set; } = null!;

        [Range(0.0, double.MaxValue, ErrorMessage = "Calories per unit must be positive")]
        public double CaloriesPerUnit { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Protein must be positive")]
        public double Protein { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Carbs must be positive")]
        public double Carbs { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Fat must be positive")]
        public double Fat { get; set; }

        public int? FoodEntityId { get; set; }
    }
}
