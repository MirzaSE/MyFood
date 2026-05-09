namespace MyFood.Application.Dtos
{
    public class IngredientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Unit { get; set; } = String.Empty;
        public double CaloriesPerUnit { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
    }
}
