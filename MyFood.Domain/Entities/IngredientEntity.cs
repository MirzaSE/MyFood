using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Domain.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Unit { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal CaloriesPerUnit { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Protein { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Carbs { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Fat { get; set; }

        // Nullable so ingredients can also live as standalone catalog entries
        public int? FoodEntityId { get; set; }

        [ForeignKey("FoodEntityId")]
        public FoodEntity? FoodEntity { get; set; }
    }
}
