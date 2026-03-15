using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Entities
{
    public class FoodEntity
    {
        public int Id { get; set; }
<<<<<<< HEAD
<<<<<<< HEAD

        [MaxLength(500)]
=======
        [MaxLength(250)]
>>>>>>> 63d6555 (Migrations and Data Seed)
=======

        [MaxLength(260)]
>>>>>>> d243566 (Assignment 1 - Ingredients API)
        public string? Name { get; set; }

        [MaxLength(50)]
        public string? Type { get; set; }

        public int Calories { get; set; }

        public DateTime Created { get; set; }

<<<<<<< HEAD
        public List<IngredientEntity> Ingredients { get; set; } = new List<IngredientEntity>();
=======
        public ICollection<IngredientEntity> Ingredients { get; set; } = new List<IngredientEntity>();
>>>>>>> d243566 (Assignment 1 - Ingredients API)
    }
}