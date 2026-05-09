namespace MyFood.Application.Dtos
{
    public class IngredientDto
    {
        public int Id {get; set; }
        public string? Name {get; set;}
        public string? Unit { get; set; }
        public int Quantity {get; set;}
        public double CaloriesPerUnit { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
    }
}
