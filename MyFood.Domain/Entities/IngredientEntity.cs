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
        public int FoodEntityId  { get; set; }
        public FoodEntity? FoodEntity  { get; set; }
        }
}