
using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodUpdateDto
    {
        [Required(ErrorMessage = "Food name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Food name must be between 2 and 100 characters")]
        public string? Name { get; set; }

        [Range(1, 50000, ErrorMessage = "Calories must be between 1 and 50000")]
        public int Calories { get; set; }

        [Required(ErrorMessage = "Food type is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Food type must be between 2 and 50 characters")]
        public string? Type { get; set; }
        public DateTime Created { get; set; }
    }
}
