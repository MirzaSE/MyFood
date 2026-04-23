using System.Collections.Generic;

namespace MyFood.Application.Services.Passwords
{
    public sealed record PasswordValidationResult(bool IsStrong, IReadOnlyCollection<string> Errors);
}
