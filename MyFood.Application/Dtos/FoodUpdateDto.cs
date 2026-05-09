using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodUpdateDto
    {
        [Required(ErrorMessage = "Food name is required")]
        [MaxLength(250, ErrorMessage = "Food name must be 250 characters or less")]
        public string? Name { get; set; }

        [Range(1, 100000, ErrorMessage = "Calories must be greater than 0")]
        public int Calories { get; set; }

        [Required(ErrorMessage = "Food type is required")]
        [MaxLength(50, ErrorMessage = "Food type must be 50 characters or less")]
        public string? Type { get; set; }
        public DateTime Created { get; set; }
    }
}
