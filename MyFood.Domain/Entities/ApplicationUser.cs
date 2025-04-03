using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = "Admin"; 
    public string LastName { get; set; } = "User";
}