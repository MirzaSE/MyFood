using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientCreateDto
    {
        [Required]
        [MaxLength(260)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Quantity { get; set; }

        [Required]
        public int FoodId { get; set; }
    }
}
