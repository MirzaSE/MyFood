using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodIngredientSelectionDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Ingredient ID must be greater than 0")]
        public int IngredientId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Ingredient quantity must be greater than 0")]
        public int Quantity { get; set; }
    }
}
