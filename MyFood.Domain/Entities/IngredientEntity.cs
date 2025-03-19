using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Application.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public int FoodEntityId { get; set; }
        
        [ForeignKey("FoodEntityId")]
        public FoodEntity FoodEntity { get; set; }
    }
}
