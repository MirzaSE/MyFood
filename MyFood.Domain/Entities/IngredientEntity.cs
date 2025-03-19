using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Entities {  
  
  public class IngredientEntity
    {
        public int Id {get; set; }

        public string? Name { get; set; }
        [MaxLength(260)]

        public int Quantity { get; set; }
        
    }
}