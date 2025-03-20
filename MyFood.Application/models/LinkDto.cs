namespace MyFood.Application.Models
{
    public class LinkDto
    {
        // Properties from MyFood.Application.LinkDto
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        // Properties from MyFood.Application.Models.LinkDto (if any)
        public string Description { get; set; } // Example additional property
        public DateTime CreatedAt { get; set; } // Example additional property
        public DateTime UpdatedAt { get; set; } // Example additional property

        // Constructor (optional)
        public LinkDto()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}