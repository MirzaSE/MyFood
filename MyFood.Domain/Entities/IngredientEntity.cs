using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }
        [MaxLength(250)]
        public string? Name { get; set; }
        public int Quantity { get; set; }
    }
}