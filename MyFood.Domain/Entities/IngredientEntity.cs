namespace MyFood.Domain.Entities
{
  public class IngredientEntity
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Quantity { get; set; }

    // Foreign key to associate the ingredient with a specific food item
    public int FoodId { get; set; }
    public FoodEntity Food { get; set; }
  }
}