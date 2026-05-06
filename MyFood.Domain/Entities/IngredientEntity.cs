using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Domain.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(30)]
        public string Unit { get; set; } = string.Empty;

        public decimal CaloriesPerUnit { get; set; }
        public decimal Protein { get; set; }
        public decimal Carbs { get; set; }
        public decimal Fat { get; set; }

        public int? FoodEntityId { get; set; }
        
        [ForeignKey("FoodEntityId")]
        public FoodEntity? FoodEntity { get; set; }
    }
}