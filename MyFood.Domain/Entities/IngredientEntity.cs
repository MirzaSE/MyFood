using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Application.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [MaxLength(260)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Quantity { get; set; } = string.Empty;

        public int FoodEntityId { get; set; }

        [ForeignKey("FoodEntityId")]
        public FoodEntity FoodEntity { get; set; } = null!;
    }
}