namespace MyFood.Application.Dtos
{
    public class CreateIngredientDto
    {
        public string? Name { get; set; }
        public string? Quantity { get; set; }
        public int FoodId { get; set; }
    }
}