using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class UpdateIngredientDto
    {
        [MaxLength(260)]
        public string? Name { get; set; }

        [MaxLength(50)]
        public string? Unit { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? CaloriesPerUnit { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Protein { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Carbs { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Fat { get; set; }

        public decimal? Quantity { get; set; }

        public int? FoodEntityId { get; set; }
    }
}