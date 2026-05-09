using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientCreateDto
    {
        [Required]
        [MaxLength(260)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Unit { get; set; } = string.Empty;
        public double CaloriesPerUnit { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
        public int FoodEntityId { get; set; }
    }
}
