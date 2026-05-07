using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientCreateDto
    {
        [Required]
        [MaxLength(260)] // Enforces the 260 character limit from the instructions
        public string? Name { get; set; }
        
        [Required]
        public string? Quantity { get; set; }

        [Required]
        public int FoodId { get; set; }
    }
}