namespace MyFood.Application.Dtos;

public class IngredientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Quantity { get; set; } = string.Empty;
    public int FoodId { get; set; }
}