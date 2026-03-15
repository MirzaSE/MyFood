using System.ComponentModel.DataAnnotations;

namespace MyFood.Domain.Entities
{
    public class IngredientEntity
    {
        public int Id { get; set; }

        [MaxLength(260)] // Requirement: 260 characters
        public string Name { get; set; } = string.Empty;

        public int Quantity { get; set; }

        // Foreign Key: Links this ingredient to a specific Food ID
        public int FoodEntityId { get; set; }

        // Navigation property
        public virtual FoodEntity? Food { get; set; }
    }
}