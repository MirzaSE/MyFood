using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(250, ErrorMessage = "Name cannot be longer than 250 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [MaxLength(50, ErrorMessage = "Type cannot be longer than 50 characters")]
        public string? Type { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Calories must be zero or greater")]
        public int Calories { get; set; }

        public ICollection<FoodIngredientCreateDto> Ingredients { get; set; } = new List<FoodIngredientCreateDto>();
    }
}
