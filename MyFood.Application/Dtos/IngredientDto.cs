namespace MyFood.Application.Dtos
{
    public class IngredientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Quantity { get; set; }
        public int FoodId { get; set; } // Foreign key reference
    }
}