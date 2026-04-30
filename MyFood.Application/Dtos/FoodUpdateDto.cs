using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodUpdateDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Calories must be a positive number")]
        public int Calories { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public string? Type { get; set; }

        public DateTime Created { get; set; }
    }
}
