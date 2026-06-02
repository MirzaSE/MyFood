using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [MaxLength(260)]
        public string? Name { get; set; }

        [MaxLength(100)]
        public string? Quantity { get; set; }

        public int FoodId { get; set; }
        public FoodEntity? Food { get; set; }
    }
}
