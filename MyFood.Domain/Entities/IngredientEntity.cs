using System.ComponentModel.DataAnnotations;

namespace MyFood.Domain.Entities;

public class IngredientEntity
{
    public int Id { get; set; }
    [StringLength(260)]
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    
    public int FoodId { get; set; }
    public FoodEntity? Food { get; set; }
}