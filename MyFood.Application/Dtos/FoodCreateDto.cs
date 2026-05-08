using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class FoodCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

<<<<<<< HEAD
        public string? Type { get; set; }

=======
        [Required(ErrorMessage = "Type is required")]
        public string? Type { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Calories must be a positive number")]
>>>>>>> origin/spring2026/assignment4/Almir.Bajric/220302201
        public int Calories { get; set; }

        public DateTime Created { get; set; }
    }
}