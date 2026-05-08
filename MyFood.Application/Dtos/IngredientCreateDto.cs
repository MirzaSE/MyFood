using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos;

public class IngredientCreateDto
{
    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(50)]
    public string? Unit { get; set; }

    [Range(0, int.MaxValue)]
    public int CaloriesPerUnit { get; set; }

    [Range(0, double.MaxValue)]
    public double Protein { get; set; }

    [Range(0, double.MaxValue)]
    public double Carbs { get; set; }

    [Range(0, double.MaxValue)]
    public double Fat { get; set; }

    public int? FoodEntityId { get; set; }
}
