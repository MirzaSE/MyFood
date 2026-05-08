using System.ComponentModel.DataAnnotations;

namespace MyFood.Domain.Entities
{
    public class FoodIngredientEntity
    {
        public int FoodEntityId { get; set; }
        public FoodEntity? FoodEntity { get; set; }

        public int IngredientEntityId { get; set; }
        public IngredientEntity? IngredientEntity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Quantity { get; set; }
    }
}
