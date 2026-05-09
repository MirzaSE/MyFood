namespace MyFood.Domain.Entities
{
    public class FoodIngredientEntity
    {
        public int Id { get; set; }

        public int FoodEntityId { get; set; }
        public FoodEntity? FoodEntity { get; set; }

        public int IngredientEntityId { get; set; }
        public IngredientEntity? Ingredient { get; set; }

        public double Quantity { get; set; }
    }
}
