namespace MyFood.Application.Dtos;

public class IngredientUpdateDto
{
     
    public string Name { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public int FoodId { get; set; }

}