using System;
using System.Collections.Generic;

namespace MyFood.Application.Dtos
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
