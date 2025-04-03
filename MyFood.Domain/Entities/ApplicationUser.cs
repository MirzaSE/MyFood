using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MyFood.Domain.Entities
{
    public class ApplicationUser : IdentityUser
 {
        // Add custom properties here


        //public string? CustomProperty { get; set; }
        public string CustomProperty { get; set; } = string.Empty;


    }
}
