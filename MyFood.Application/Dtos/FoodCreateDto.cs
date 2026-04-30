using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [MinLength(2, ErrorMessage = "Type must be at least 2 characters")]
        public string? Type { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Calories must be greater than 0")]
        public int Calories { get; set; }
        public DateTime Created { get; set; }
    }
}
