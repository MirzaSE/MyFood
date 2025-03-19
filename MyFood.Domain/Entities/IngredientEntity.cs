using MyFood.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFood.Domain.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }  // Primary Key
        public string Name { get; set; } = string.Empty; // Ingredient name
        public string Unit { get; set; } = string.Empty; // (e.g., grams, ml, pieces)
        public int FoodEntityId { get; set; } // Foreign Key to FoodEntity
        public FoodEntity? Food { get; set; } // Navigation property
    }
}

