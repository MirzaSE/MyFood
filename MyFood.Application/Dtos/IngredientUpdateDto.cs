using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos;

public class IngredientUpdateDto
{
    [MaxLength(260)]
    public string? Name { get; set; }

    [Range(1, int.MaxValue)]
    public int? Quantity { get; set; }

    [MaxLength(50)]
    public string? Unit { get; set; }

    [Range(0.0001, double.MaxValue, ErrorMessage = "Calories per unit must be greater than zero.")]
    public decimal? CaloriesPerUnit { get; set; }

    [Range(0.0001, double.MaxValue, ErrorMessage = "Protein must be greater than zero.")]
    public decimal? Protein { get; set; }

    [Range(0.0001, double.MaxValue, ErrorMessage = "Carbs must be greater than zero.")]
    public decimal? Carbs { get; set; }

    [Range(0.0001, double.MaxValue, ErrorMessage = "Fat must be greater than zero.")]
    public decimal? Fat { get; set; }
}
