using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Entities
{
    public class FoodEntity
    {
        public int Id { get; set; }
<<<<<<< HEAD

        [MaxLength(500)]
=======
        [MaxLength(250)]
>>>>>>> 63d6555 (Migrations and Data Seed)
        public string? Name { get; set; }

        [MaxLength(50)]
        public string? Type { get; set; }

        public int Calories { get; set; }

        public DateTime Created { get; set; }

        public List<IngredientEntity> Ingredients { get; set; } = new List<IngredientEntity>();
    }
}
