using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientUpdateDto
    {
        [MaxLength(100, ErrorMessage = "Ingredient name must be 100 characters or less")]
        public string? Name { get; set; }

        [MaxLength(50, ErrorMessage = "Unit must be 50 characters or less")]
        public string? Unit { get; set; }

        public decimal? CaloriesPerUnit { get; set; }

        public decimal? Protein { get; set; }

        public decimal? Carbs { get; set; }

        public decimal? Fat { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "FoodEntityId must be greater than 0")]
        public int? FoodId { get; set; }
    }
}
