using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Domain.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Quantity { get; set; } 

        public int? FoodId { get; set; }
        
        [ForeignKey("FoodId")]
        public FoodEntity? FoodEntity { get; set; }
    }
}