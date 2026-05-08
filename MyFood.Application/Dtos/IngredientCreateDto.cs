using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientCreateDto
    {
        [Required]
        [MaxLength(260)]
        public string? Name { get; set; }
        public int Quantity { get; set; }
        [Required]
        public int FoodEntityId { get; set; }
    }
}
