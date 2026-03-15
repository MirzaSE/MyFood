using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Application.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }
        [MaxLength(260)]
        public string? Name { get; set; }
        public double Quantity { get; set; }
        public string? Type { get; set; }
        public int Calories { get; set; }
        public DateTime Created { get; set; }

        [ForeignKey("FoodId")]
        public FoodEntity? Food { get; set; }
        public int FoodId { get; set; }
    }
}