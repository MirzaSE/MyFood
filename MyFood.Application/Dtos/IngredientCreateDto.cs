using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        [MaxLength(50, ErrorMessage = "Unit cannot be longer than 50 characters")]
        public string? Unit { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Calories must be zero or greater")]
        public decimal CaloriesPerUnit { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Protein must be zero or greater")]
        public decimal Protein { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Carbs must be zero or greater")]
        public decimal Carbs { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Fat must be zero or greater")]
        public decimal Fat { get; set; }
    }
}
