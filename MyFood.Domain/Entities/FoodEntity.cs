using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Entities
{
    public class FoodEntity
    {
        public int Id { get; set; }
        [MaxLength(500)]
        public string? Name { get; set; }
        [MaxLength(50)]
        public string? Type { get; set; }
        public int Calories { get; set; }
        public DateTime Created { get; set; }
    }
}
