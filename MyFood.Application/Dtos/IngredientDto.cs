namespace MyFood.Application.Dtos
{
    public class IngredientDto
    {
        public string? Name { get; set; }
        public int Id { get; set; }
        public double Quantity { get; set; }
        public string? Type { get; set; }
        public int Calories { get; set; }
        public DateTime Created { get; set; }
        public int FoodId { get; set; }
    }
}