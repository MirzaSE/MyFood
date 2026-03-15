using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }
        public int FoodId { get; set; }

        [MaxLength(260)]
        public string Name { get; set; } = string.Empty;

        public string Quantity { get; set; } = string.Empty;

        public FoodEntity? Food { get; set; }
    }
}
