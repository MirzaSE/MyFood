namespace MyFood.Application.Dtos
{
    public class IngredientDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Quantity { get; set; }
        
        // Tells the client which Food item this ingredient belongs to
        public int FoodId { get; set; } 
    }
}