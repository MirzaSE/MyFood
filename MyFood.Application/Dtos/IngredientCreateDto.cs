using System.ComponentModel.DataAnnotations;
namespace MyFood.Application.Dtos
{
    public class IngredientCreateDto
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(260)]
        public string? Name { get; set; }
        public double Quantity { get; set; }
        public string? Type { get; set; }
        public int Calories { get; set; }
        public DateTime Created { get; set; }
        public int FoodId { get; set; }
    }
}