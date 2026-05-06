using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientUpdateDto
    {
        [StringLength(100, MinimumLength = 1)]
        public string? Name { get; set; }

        [StringLength(30, MinimumLength = 1)]
        public string? Unit { get; set; }

        [Range(0.0001, 100000)]
        public decimal? CaloriesPerUnit { get; set; }

        [Range(0, 100000)]
        public decimal? Protein { get; set; }

        [Range(0, 100000)]
        public decimal? Carbs { get; set; }

        [Range(0, 100000)]
        public decimal? Fat { get; set; }
    }
}
