namespace MyFood.Api.Models
{
    public class JwtSettings
    {
        public required string ValidAudience { get; set; }
        public required string ValidIssuer { get; set; }
        public required string Secret { get; set; }
        public int TokenValidityInMinutes { get; set; }
        public int RefreshTokenValidityInDays { get; set; }
    }
}