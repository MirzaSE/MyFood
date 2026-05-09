using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientCreateDto
    {
        [Required(ErrorMessage = "Ingredient name is required")]
        [MaxLength(100, ErrorMessage = "Ingredient name must be 100 characters or less")]
        public string? Name { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "FoodEntityId must be greater than 0")]
        public int FoodId { get; set; }
    }
}
