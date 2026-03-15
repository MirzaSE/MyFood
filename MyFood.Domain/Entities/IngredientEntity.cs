using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Application.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(260)]
        public string? Name { get; set; }

        [Required]
        public string? Quantity { get; set; }

        [ForeignKey(nameof(Food))]
        public int FoodEntityId { get; set; }

        public FoodEntity? Food { get; set; }
    }
}
