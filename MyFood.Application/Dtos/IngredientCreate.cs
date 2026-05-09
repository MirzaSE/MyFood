namespace MyFood.Application.Dtos
{
    public class IngredientCreateDto
    {
        public string? Name { get; set; }
        public DateTime Created { get; set; }
        
        public string? Quantity { get; set; }
    }
}
