using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodUpdateDto
    {
        [MaxLength(250, ErrorMessage = "Food name cannot exceed 250 characters")]
        public string? Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Calories must be a non-negative number")]
        public int Calories { get; set; }

        [MaxLength(50, ErrorMessage = "Food type cannot exceed 50 characters")]
        public string? Type { get; set; }

        public DateTime Created { get; set; }
    }
}
