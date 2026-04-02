using System;
using System.Collections.Generic;
using System.Linq;

namespace MyFood.Application.Services.Passwords
{
    public class PasswordService : IPasswordService
    {
        private const int MinimumLength = 8;

        public string HashPassword(string password)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(password);
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        public PasswordValidationResult ValidateStrength(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Password cannot be empty or whitespace.");
            }
            else
            {
                if (password.Length < MinimumLength)
                {
                    errors.Add($"Password must be at least {MinimumLength} characters long.");
                }

                if (!password.Any(char.IsUpper))
                {
                    errors.Add("Password must contain at least one uppercase letter.");
                }

                if (!password.Any(char.IsLower))
                {
                    errors.Add("Password must contain at least one lowercase letter.");
                }

                if (!password.Any(char.IsDigit))
                {
                    errors.Add("Password must contain at least one digit.");
                }

                if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                {
                    errors.Add("Password must contain at least one special character.");
                }

                if (password.Any(char.IsWhiteSpace))
                {
                    errors.Add("Password cannot contain whitespace characters.");
                }
            }

            return new PasswordValidationResult(errors.Count == 0, errors);
        }
    }
}
