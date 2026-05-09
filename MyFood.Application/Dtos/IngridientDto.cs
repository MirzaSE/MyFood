namespace MyFood.Application.Dtos;

public class IngridientDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string Unit { get; set; } = "g";
    public double CaloriesPerUnit { get; set; }
    public double Protein { get; set; }
    public double Carbs { get; set; }
    public double Fat { get; set; }
}