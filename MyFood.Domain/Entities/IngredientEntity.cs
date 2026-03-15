using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }
        [MaxLength(260)]
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public int FoodEntityId { get; set; }
        public FoodEntity? Food { get; set; }
    }
}
