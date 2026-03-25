using Microsoft.AspNetCore.Identity;

namespace myfood.domain.entities
 {
public class ApplicationUser : IdentityUser

{
    public string FullName { get; set; } = string.Empty;

}
 }