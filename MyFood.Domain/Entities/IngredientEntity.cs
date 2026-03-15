using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Entities
{
   public class IngredientEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Quantity { get; set; } = string.Empty;

    public int FoodId { get; set; }

    public FoodEntity? Food { get; set; }
}
}
