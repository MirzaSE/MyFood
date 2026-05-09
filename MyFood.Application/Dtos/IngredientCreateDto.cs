using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos;

public class IngredientCreateDto
{
    [Required]
    [MaxLength(260)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Unit { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "CaloriesPerUnit must be greater than 0")]
    public double CaloriesPerUnit { get; set; }

    [Range(0, double.MaxValue)]
    public double Protein { get; set; }

    [Range(0, double.MaxValue)]
    public double Carbs { get; set; }

    [Range(0, double.MaxValue)]
    public double Fat { get; set; }

    public int? FoodEntityId { get; set; }
}
