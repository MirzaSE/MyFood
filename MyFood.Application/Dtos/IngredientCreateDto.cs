using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos;

public class IngredientCreateDto
{
    [Required]
    [MaxLength(260)]
    public string Name { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;

    [Required]
    [MaxLength(50)]
    public string Unit { get; set; } = string.Empty;

    [Range(0.0001, double.MaxValue, ErrorMessage = "Calories per unit must be greater than zero.")]
    public decimal CaloriesPerUnit { get; set; }

    [Range(0.0001, double.MaxValue, ErrorMessage = "Protein must be greater than zero.")]
    public decimal Protein { get; set; }

    [Range(0.0001, double.MaxValue, ErrorMessage = "Carbs must be greater than zero.")]
    public decimal Carbs { get; set; }

    [Range(0.0001, double.MaxValue, ErrorMessage = "Fat must be greater than zero.")]
    public decimal Fat { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A valid food item is required.")]
    public int? FoodEntityId { get; set; }
}
