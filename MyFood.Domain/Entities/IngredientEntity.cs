using System.ComponentModel.DataAnnotations;

namespace MyFood.Domain.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [MaxLength(260)]
        public string Name { get; set; } = null!;

        public string Quantity { get; set; } = null!;

        public int FoodId { get; set; }

        public FoodEntity? Food { get; set; }
    }
}