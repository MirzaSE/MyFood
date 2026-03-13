using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientUpdateDto
    {
        [Required]
        [MaxLength(260)]
        public string? Name { get; set; }
        public string? Quantity { get; set; }
        public int FoodEntityId { get; set; }
    }
}