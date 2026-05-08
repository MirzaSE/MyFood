namespace MyFood.Application.Dtos;

public class IngredientUpdateDto
{
    public string? Name { get; set; }
    public string? Unit { get; set; }
    public int CaloriesPerUnit { get; set; }
    public double Protein { get; set; }
    public double Carbs { get; set; }
    public double Fat { get; set; }
    public int FoodEntityId { get; set; }
}
