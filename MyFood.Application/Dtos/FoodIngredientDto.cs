using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodIngredientDto
    {
        [Required]
        public int IngredientId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public decimal Quantity { get; set; }
    }
}