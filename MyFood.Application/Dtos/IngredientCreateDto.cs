using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientCreateDto
    {
        [Required]
        [MaxLength(260)]
        public string Name { get; set; } = null!;

        public string? Quantity { get; set; }

        [Required]
        public int FoodId { get; set; } // Foreign key to Food
    }
}