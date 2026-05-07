using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Domain.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(260)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Unit { get; set; } = string.Empty;

        public decimal CaloriesPerUnit { get; set; }

        public decimal Protein { get; set; }

        public decimal Carbs { get; set; }

        public decimal Fat { get; set; }

        public decimal? Quantity { get; set; }

        public int? FoodEntityId { get; set; }

        [ForeignKey(nameof(FoodEntityId))]
        public FoodEntity? FoodEntity { get; set; }
    }
}