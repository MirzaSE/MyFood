using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        public string? Unit { get; set; }

        [Range(0.0001, double.MaxValue, ErrorMessage = "Calories per unit must be greater than zero")]
        public double CaloriesPerUnit { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Protein must be zero or greater")]
        public double Protein { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Carbs must be zero or greater")]
        public double Carbs { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Fat must be zero or greater")]
        public double Fat { get; set; }
    }
}
