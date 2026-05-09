using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientCreateDto
    {
        [Required]
        [MaxLength(260)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Unit { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public double CaloriesPerUnit { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double Protein { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double Carbs { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double Fat { get; set; }
    }
}
