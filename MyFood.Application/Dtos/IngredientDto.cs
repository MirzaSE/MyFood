using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFood.Application.Dtos
{
    public class IngredientDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Quantity { get; set; }
    }
}