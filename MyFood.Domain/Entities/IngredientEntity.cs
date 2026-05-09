using System.ComponentModel.DataAnnotations;

namespace MyFood.Domain.Entities;

public class IngredientEntity
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string Unit { get; set; } = "g";

    public double CaloriesPerUnit { get; set; }

    public double Protein { get; set; }

    public double Carbs { get; set; }

    public double Fat { get; set; }
}