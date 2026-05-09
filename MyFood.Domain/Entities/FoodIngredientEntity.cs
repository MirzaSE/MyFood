namespace MyFood.Domain.Entities
{
    public class FoodIngredientEntity
    {
        public int FoodEntityId { get; set; }
        public FoodEntity Food { get; set; } = null!;

        public int IngredientEntityId { get; set; }
        public IngredientEntity Ingredient { get; set; } = null!;

        public decimal Quantity { get; set; }
    }
}
