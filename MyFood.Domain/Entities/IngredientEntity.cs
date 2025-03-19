using MyFood.Application.Entities;


namespace MyFood.Api.Data.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Quantity { get; set; }

        public int FoodId { get; set; }
        public FoodEntity Food { get; set; }
    }
}