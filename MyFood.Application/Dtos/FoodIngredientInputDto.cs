using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodIngredientInputDto
    {
        [Range(1, int.MaxValue)]
        public int IngredientId { get; set; }

        [Range(0.01, 100000)]
        public decimal Quantity { get; set; }
    }
}
