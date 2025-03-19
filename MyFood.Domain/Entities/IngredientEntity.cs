using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MyFood.Application.Entities
{
    [Table("Ingredients")]
    public class IngredientEntity
    {
        public int Id { get; set; }
        [MaxLength(260)]
        public string Name { get; set; } 
        public int Quantity { get; set; } 
        public int FoodEntityId { get; set; } 
     
    }
}
