using Microsoft.AspNetCore.Identity;

namespace MyFood.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}
