namespace MyFood.Application.Dtos
{
    public class FoodDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public int Calories { get; set; }
        public DateTime Created { get; set; }
        public List<FoodIngredientDto> Ingredients { get; set; } = new();
    }

    public class FoodIngredientDto
    {
        public int IngredientId { get; set; }
        public string IngredientName { get; set; } = String.Empty;
        public string Unit { get; set; } = String.Empty;
        public double Quantity { get; set; }
        public double CaloriesPerUnit { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
    }
}
