using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public string? Type { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Calories must be zero or greater")]
        public int Calories { get; set; }

        public DateTime Created { get; set; }
    }
}
