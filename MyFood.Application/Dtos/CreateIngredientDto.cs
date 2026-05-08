namespace MyFood.Application.Dtos
{
    public class CreateIngredientDto
    {
        public string? Name { get; set; }
        public int FoodEntityId { get; set; }
    }
}