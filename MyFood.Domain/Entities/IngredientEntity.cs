using System;
using System.ComponentModel.DataAnnotations;
using MyFood.Application.Entities;

namespace MyFood.Domain.Entities;

public class IngredientEntity
{
        public int Id { get; set; }
        [MaxLength(260)]
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public int FoodEntityId { get; set; }
        public FoodEntity? Food { get; set; }
}
