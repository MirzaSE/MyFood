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
        public string? fullName {  get; set; }
        public string? favouriteMeal { get; set; }
        public int spiceTolerance { get; set; }
    }
}
