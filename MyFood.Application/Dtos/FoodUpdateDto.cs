using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodUpdateDto
    {
        [Required]
        [MaxLength(250)]
        public string Name { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Calories must be greater than zero.")]
        public int Calories { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;
    }
}
