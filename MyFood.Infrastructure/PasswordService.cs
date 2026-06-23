using System.Security.Cryptography;
using MyFood.Application.Services;

public class PasswordService : IPasswordService
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private const char SegmentDelimiter = ';';

    public string HashPassword(string password)
    {
        var salt = new byte[SaltSize];
        RandomNumberGenerator.Fill(salt);

        using var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var hash = deriveBytes.GetBytes(KeySize);

        return string.Join(SegmentDelimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash), Iterations);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var segments = hashedPassword.Split(SegmentDelimiter);
        if (segments.Length != 3) return false;

        var salt = Convert.FromBase64String(segments[0]);
        var hash = Convert.FromBase64String(segments[1]);
        if (!int.TryParse(segments[2], out var iterations)) return false;

        using var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        var computedHash = deriveBytes.GetBytes(hash.Length);

        return CryptographicOperations.FixedTimeEquals(computedHash, hash);
    }
}