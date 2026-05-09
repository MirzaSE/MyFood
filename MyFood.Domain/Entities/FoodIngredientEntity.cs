using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Domain.Entities
{
    public class FoodIngredientEntity
    {
        public int Id { get; set; }
        public int FoodEntityId { get; set; }
        public int IngredientEntityId { get; set; }
        public double Quantity { get; set; }

        [ForeignKey("FoodEntityId")]
        public FoodEntity? FoodEntity { get; set; }

        [ForeignKey("IngredientEntityId")]
        public IngredientEntity? Ingredient { get; set; }
    }
}
