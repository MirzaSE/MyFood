using System.ComponentModel.DataAnnotations;


namespace MyFood.Application.Dtos
{
    public class IngredientCreateDto
    {
        [Required]
        public string? Name { get; set; }
        
        public int Quantity { get; set; }
        
    }
}
