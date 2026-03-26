using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientUpdateDto
    {
        [MaxLength(260)]
        public string Name { get; set; }

        public string Quantity { get; set; }

        public int FoodId { get; set; }
    }
}
