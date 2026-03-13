using System.ComponentModel.DataAnnotations;
namespace MyFood.Application.Dtos
{
    public class IngredientUpdateDto
    {
        public string Name { get; set; } = null!;
        public int Quantity { get; set; }
    }
}