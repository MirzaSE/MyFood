using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(250, ErrorMessage = "Name cannot exceed 250 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [MaxLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string? Type { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Calories must be a positive number")]
        public int Calories { get; set; }

        public DateTime Created { get; set; }

        public List<FoodIngredientCreateDto>? Ingredients { get; set; }
    }

    public class FoodIngredientCreateDto
    {
        [Range(1, int.MaxValue)]
        public int IngredientId { get; set; }

        [Range(0.01, double.MaxValue)]
        public double Quantity { get; set; }
    }
}
