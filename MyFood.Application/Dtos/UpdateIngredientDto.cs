using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class UpdateIngredientDto
    {
        [MaxLength(260)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Quantity { get; set; } = string.Empty;

        public int FoodEntityId { get; set; }
    }
}