using System.ComponentModel.DataAnnotations;
using MyFood.Domain.Entities;

namespace MyFood.Application.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string? Name { get; set; }
        [MaxLength(260)]
        public int Quantity { get; set; }
        public int? FoodEntityId { get; set; }
        public FoodEntity? FoodEntity { get; set; }
    }
}