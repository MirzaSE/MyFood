using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodCreateDto
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Type { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Calories must be greater than zero")]
        public int Calories { get; set; }

        public DateTime Created { get; set; }

        public List<FoodIngredientDto>? Ingredients { get; set; }
    }
}
