using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Application.Entities
{
    public class IngredientEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(260)]
        public string Name { get; set; } = string.Empty;

        public string? Quantity { get; set; }

        public int FoodId { get; set; }
        
        [ForeignKey("FoodId")]
        public FoodEntity? Food { get; set; }
    }
}