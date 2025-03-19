using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }
        [MaxLength(250)]
        public string? Name { get; set; }
        [MaxLength(260)]
        public string? Type { get; set; }
        public int? FoodEntityId { get; set; }
        public FoodEntity FoodEntity { get; set; }
        
    }
}