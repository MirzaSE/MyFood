using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Domain.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Unit { get; set; } = null!;

        public double CaloriesPerUnit { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }

        // Nullable so an ingredient can stand alone in the catalog;
        // deleting a food sets the FK back to null instead of cascading.
        public int? FoodEntityId { get; set; }

        [ForeignKey("FoodEntityId")]
        public FoodEntity? FoodEntity { get; set; }
    }
}
