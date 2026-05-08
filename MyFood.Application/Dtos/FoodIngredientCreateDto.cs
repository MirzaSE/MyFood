using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodIngredientCreateDto
    {
        [Required]
        public int IngredientId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Quantity must be zero or greater")]
        public decimal Quantity { get; set; }
    }
}
