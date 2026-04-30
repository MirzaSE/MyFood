using System.ComponentModel.DataAnnotations;

public class FoodCreateDto
{
    [Required]
    [MinLength(2)]
    public string? Name { get; set; }

    [Required]
    public string? Type { get; set; }

    [Range(0, 5000)]
    public int Calories { get; set; }

    public DateTime Created { get; set; }
}
