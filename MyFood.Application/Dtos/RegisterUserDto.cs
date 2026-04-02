using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFood.Application.Dtos
{
    public class RegisterUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
    }
}