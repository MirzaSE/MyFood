namespace MyFood.Domain.Models
{
public class JwtSettings
{
    public string ValidAudience { get; set; }
    public string ValidIssuer { get; set; }
    public string Secret { get; set; }
    public int ExpiryInHours { get; set; }
}
}