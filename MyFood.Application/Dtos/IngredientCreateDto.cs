using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientCreateDto
    {
        [Required]
        [MaxLength(260)]
        public string Name { get; set; } = null!;

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public int Food_Id { get; set; }
    }
}
