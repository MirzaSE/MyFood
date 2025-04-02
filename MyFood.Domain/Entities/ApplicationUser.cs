using Microsoft.AspNetCore.Identity;

namespace MyFood.Infrastructure.Repositories
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
 
