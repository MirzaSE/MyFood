using System.Web.Helpers;

namespace MyFood.Api
{
    public class JwtSettings
    {
        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }

        public string Secret { get; set; }


    }
}
