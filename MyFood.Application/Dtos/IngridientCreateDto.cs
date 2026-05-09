using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos;

public class IngridientCreateDto
{
    [MaxLength(100)]
    [Required]    
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Unit { get; set; } = "g";
    
    [Required]
    public double CaloriesPerUnit { get; set; }
    
    [Required]
    public double Protein { get; set; }
    
    [Required]
    public double Carbs { get; set; }
    
    [Required]
    public double Fat { get; set; }
}