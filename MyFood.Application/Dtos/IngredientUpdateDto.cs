namespace MyFood.Application.Dtos
{
    public class IngredientUpdateDto
    {
        public string? Name { get; set; }
        public string? Unit { get; set; }
        public decimal? CaloriesPerUnit { get; set; }
        public decimal? Protein { get; set; }
        public decimal? Carbs { get; set; }
        public decimal? Fat { get; set; }
    }
}
