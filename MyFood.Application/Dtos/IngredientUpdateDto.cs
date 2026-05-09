using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientUpdateDto
    {
        [Required]
        public string? Name { get; set; }
        
        [Required]
        public string? Quantity { get; set; }

        [Required]
        public int? FoodId { get; set; }
    }
}