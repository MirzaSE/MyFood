using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MyFood.Domain.Entities;

namespace MyFood.Application.Entities
{
    public class FoodEntity
    {
        public int Id { get; set; }

        [MaxLength(250)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Type { get; set; }

        public int Calories { get; set; }

        public DateTime Created { get; set; }
        public List<IngredientEntity> Ingredients { get; set; } = new();
    }
}
