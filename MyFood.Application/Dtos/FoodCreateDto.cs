using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodCreateDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters.")]
        [MaxLength(250, ErrorMessage = "Name cannot exceed 250 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Type is required.")]
        [MinLength(2, ErrorMessage = "Type must be at least 2 characters.")]
        [MaxLength(50, ErrorMessage = "Type cannot exceed 50 characters.")]
        public string Type { get; set; } = string.Empty;

        [Range(0, 10000, ErrorMessage = "Calories must be between 0 and 10000.")]
        public int Calories { get; set; }
    }
}
