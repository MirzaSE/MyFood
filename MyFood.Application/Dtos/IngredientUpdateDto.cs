using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    /// <summary>
    /// All fields are optional so the same DTO can drive a partial (PATCH-like) update.
    /// </summary>
    public class IngredientUpdateDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(20)]
        public string? Unit { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Calories per unit must be greater than 0")]
        public decimal? CaloriesPerUnit { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Protein must be a positive number")]
        public decimal? Protein { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Carbs must be a positive number")]
        public decimal? Carbs { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Fat must be a positive number")]
        public decimal? Fat { get; set; }

        public int? FoodEntityId { get; set; }
    }
}
