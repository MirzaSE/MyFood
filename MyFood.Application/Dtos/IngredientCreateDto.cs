using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientCreateDto
    {
        [Required]
        [MaxLength(260)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Quantity { get; set; } = string.Empty;
    }
}
