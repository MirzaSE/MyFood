using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodUpdateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(1, ErrorMessage = "Name is required")]
        public required string Name { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Calories must be greater than 0")]
        public int Calories { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [MinLength(1, ErrorMessage = "Type is required")]
        public required string Type { get; set; }
        public DateTime Created { get; set; }
    }
}
