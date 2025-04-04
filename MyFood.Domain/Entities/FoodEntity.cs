using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFood.Application.Entities
{
    public class FoodEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(250)]
        public string? Name { get; set; }

        [MaxLength(50)]
        public string? Type { get; set; }

        public int Calories { get; set; }

        public DateTime Created { get; set; }
    }
}