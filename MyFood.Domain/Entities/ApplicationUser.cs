using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;


namespace MyFood.Application.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(50)] 
        public string? FullName { get; set; }
              
    }
}