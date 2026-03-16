using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Application.Entities
{
    public class IngredientEntity 
    {
        public int Id { get; set; }

        [MaxLength(260)]
        public string? Name { get; set; }

        public int Quantity { get; set; }

        // Foreign key linking to FoodEntity
        [ForeignKey("foodItem")]
        public int Food_Id { get; set; }

        public FoodEntity foodItem { get; set; } = null!;
    }
}