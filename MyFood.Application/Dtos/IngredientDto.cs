namespace MyFood.Application.Dtos;

public class IngredientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public int FoodId { get; set; }
}