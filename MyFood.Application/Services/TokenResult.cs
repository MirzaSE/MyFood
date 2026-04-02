using System;

namespace MyFood.Application.Services
{
    public record TokenResult(string Token, DateTime ExpiresAt);
}
