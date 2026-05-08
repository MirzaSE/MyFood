using System.ComponentModel.DataAnnotations;
<<<<<<< HEAD
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Domain.Entities
=======

namespace MyFood.Application.Entities
>>>>>>> origin/spring2026/assignment1/almir.bajric/220302201
{
    public class IngredientEntity
    {
        public int Id { get; set; }

<<<<<<< HEAD
        [MaxLength(100)]
        public string Name { get; set; }

        public int FoodEntityId { get; set; }
        
        [ForeignKey("FoodEntityId")]
        public FoodEntity FoodEntity { get; set; }
=======
        [MaxLength(260)]
        public string? Name { get; set; }

        public string? Quantity { get; set; }

        public int FoodId { get; set; }

        public FoodEntity? Food { get; set; }
>>>>>>> origin/spring2026/assignment1/almir.bajric/220302201
    }
}