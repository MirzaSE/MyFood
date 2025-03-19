namespace MyFood.Application.Dtos;

public class IngredientCreateDto
{
    public string? Name { get; set; }
    public string Quantity { get; set; } = string.Empty;
    public int FoodEntityId { get; set; }
}