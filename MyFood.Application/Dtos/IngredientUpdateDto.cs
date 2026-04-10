namespace MyFood.Application.Dtos;

public class IngredientUpdateDto
{
    public string? Name { get; set; }
    public int Quantity { get; set; }
    public int? FoodEntityId { get; set; }
}