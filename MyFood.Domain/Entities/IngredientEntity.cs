using System.ComponentModel.DataAnnotations;

namespace MyFood.Domain.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [MaxLength(260)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Unit { get; set; } = string.Empty;
        public double CaloriesPerUnit { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }

        public int FoodEntityId { get; set; }
        public FoodEntity? FoodEntity { get; set; }
    }
}
