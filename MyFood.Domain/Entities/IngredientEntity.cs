using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace MyFood.Application.Entities
{
    public class IngredientEntity
    {
        public int Id {get; set;}

        public int Name {get; set;}
        [MaxLength(260)]

        public int Quantity {get; set;}
    }
}
