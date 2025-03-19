using MyFood.Application.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFood.Application.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [MaxLength(260)]
        public string? Quantity { get; set; }
        public int FoodEntityId { get; set; }
        public FoodEntity FoodEntity { get; set; }
    }
}
