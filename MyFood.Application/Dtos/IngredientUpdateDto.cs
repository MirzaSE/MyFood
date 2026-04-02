using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos
{
    public class IngredientUpdateDto
    {
        [MaxLength(260)]
        public string? Name { get; set; }

        [Range(0, int.MaxValue)]
        public int? Quantity { get; set; }

        public int? Food_Id { get; set; }
    }
}
