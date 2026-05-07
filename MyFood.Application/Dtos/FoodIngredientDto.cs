using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos;

public class FoodIngredientDto
{
    public int? Id { get; set; }

    [MaxLength(100)]
    public string? Name { get; set; }

    public int? Quantity { get; set; }

    public string? Unit { get; set; }

    public decimal? CaloriesPerUnit { get; set; }
}
