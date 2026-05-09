using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodCreateDto
    {
        [Required(ErrorMessage = "Food name is required")]
        [StringLength(100, ErrorMessage = "Food name cannot be longer than 100 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Food type is required")]
        [StringLength(100, ErrorMessage = "Food type cannot be longer than 100 characters")]
        public string? Type { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Calories must be greater than 0")]
        public int Calories { get; set; }

        public DateTime Created { get; set; }
    }
}
