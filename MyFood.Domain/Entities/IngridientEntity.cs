using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Application.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [MaxLength(260)]
        public string? Name { get; set; }

        public string? Quantity { get; set; }

        // Foreign Key to FoodEntity
        public int FoodEntityId { get; set; }

        [ForeignKey("FoodEntityId")]
        public FoodEntity Food { get; set; }
    }
}
