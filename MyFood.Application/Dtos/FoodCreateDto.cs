using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Name cannot be empty")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Type cannot be empty")]
        public string? Type { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Calories must be greater than 0")]
        public int Calories { get; set; }

        public double Protein { get; set; }

        public double Carbs { get; set; }

        public double Fat { get; set; }

        public List<FoodIngredientSelectionDto> Ingredients { get; set; } = new();

    }
}
