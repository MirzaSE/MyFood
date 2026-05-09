using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Domain.Entities
{
    public class FoodIngredient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FoodId { get; set; }

        [Required]
        public int IngredientId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        // Navigation properties
        [ForeignKey("FoodId")]
        public FoodEntity? Food { get; set; }

        [ForeignKey("IngredientId")]
        public IngredientEntity? Ingredient { get; set; }
    }
}