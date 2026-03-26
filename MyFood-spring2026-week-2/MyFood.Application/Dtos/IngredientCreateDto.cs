namespace MyFood.Application.Dtos
{
    public class IngredientCreateDto
    {
        public string? Name { get; set; }
        public string? Quantity { get; set; }
        public int FoodId { get; set; }
    }
}
