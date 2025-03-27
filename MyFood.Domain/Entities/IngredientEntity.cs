using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFood.Domain.Entities
{
    public class IngredientEntity   {
        public int Id { get; set; }

        [MaxLength(260)] 
        public string Name { get; set; }

        [MaxLength(50)]
        public string Quantity { get; set; }



    }
}
