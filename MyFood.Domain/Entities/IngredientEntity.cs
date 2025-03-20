using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Domain.Entities
{
    public class IngredientEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(260)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Quantity { get; set; } = string.Empty;

        [ForeignKey("FoodEntity")]
        public int FoodId { get; set; }
        public FoodEntity? FoodEntity { get; set; }
    }
}