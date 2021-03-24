using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace ToDoList.WebApi.Jwt
{
    public class JwtTokenConfig
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
    }
}
