using Microsoft.AspNetCore.Identity;

namespace MyFood.Domain;

public class ApplicationUser: IdentityUser
{
	public string FullName { get; set; } = String.Empty;
}
