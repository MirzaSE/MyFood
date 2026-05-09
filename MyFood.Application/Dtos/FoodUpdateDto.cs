using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodUpdateDto
    {
        [Required(ErrorMessage = "Food name is required")]
        [StringLength(100, ErrorMessage = "Food name cannot be longer than 100 characters")]
        public string? Name { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Calories must be greater than 0")]
        public int Calories { get; set; }

        [Required(ErrorMessage = "Food type is required")]
        [StringLength(100, ErrorMessage = "Food type cannot be longer than 100 characters")]
        public string? Type { get; set; }

        public DateTime Created { get; set; }
    }
}
