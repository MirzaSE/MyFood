using System.ComponentModel.DataAnnotations;

namespace MyFood.Domain.Entities
{
    public class FoodEntity
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string? Name { get; set; }
        [MaxLength(50)]
        public string? Type { get; set; }
        public int Calories { get; set; }
        public DateTime Created { get; set; }
<<<<<<< HEAD
        public List<IngredientEntity> Ingredients { get; set; } = new List<IngredientEntity>();
=======
        public ICollection<IngredientEntity> Ingredients { get; set; } = new List<IngredientEntity>();
>>>>>>> origin/spring2026/assignment1/almir.bajric/220302201
    }
}
