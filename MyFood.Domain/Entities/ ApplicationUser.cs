using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    // Dodaj prilagođena svojstva
    public string CustomProperty { get; set; }
}

