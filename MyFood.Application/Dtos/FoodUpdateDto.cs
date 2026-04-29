
using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodUpdateDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(250, ErrorMessage = "Name cannot be longer than 250 characters.")]
        public string? Name { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Calories must be greater than 0.")]
        public int Calories { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        [MaxLength(50, ErrorMessage = "Type cannot be longer than 50 characters.")]
        public string? Type { get; set; }

        public DateTime Created { get; set; }
    }
}
