
using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodUpdateDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Type is required")]
        public string? Type { get; set; }
        [Required(ErrorMessage = "Calories is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Calories must be 0 or greater")]
        public int Calories { get; set; }
    }
}
