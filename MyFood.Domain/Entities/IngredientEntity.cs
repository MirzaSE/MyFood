using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Domain.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [MaxLength(260)]
        public string? Name { get; set; }

        public int Quantity { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }

        public int FoodEntityId { get; set; }

        [ForeignKey("FoodEntityId")]
        public FoodEntity? FoodEntity { get; set; }
    }
}