using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Domain.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [MaxLength(260)]
        public string? Name { get; set; }

        public string? Quantity { get; set; }

        public int FoodId { get; set; }

        [ForeignKey(nameof(FoodId))]
        public FoodEntity? Food { get; set; }
    }
}