namespace MyFood.Application.Dtos
{
    public class IngredientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public int Quantity { get; set; }
        public int FoodEntityId { get; set; }
    }
}
