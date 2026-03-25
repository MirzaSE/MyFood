using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace MyFood.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(500)]
        public string? FullName {get; set;}
    }
}
