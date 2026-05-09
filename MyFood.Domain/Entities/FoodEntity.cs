using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFood.Domain.Entities
{
    public class FoodEntity
    {
        public int Id { get; set; }

        [MaxLength(500)]
        public string? Name { get; set; }

        [MaxLength(50)]
        public string? Type { get; set; }

        public int Calories { get; set; }

        public double Protein { get; set; }

        public double Carbs { get; set; }

        public double Fat { get; set; }

        public DateTime Created { get; set; }

        public List<IngredientEntity> Ingredients { get; set; } = new();
    }
}
