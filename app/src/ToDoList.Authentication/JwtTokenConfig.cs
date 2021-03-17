using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace ToDoList.Authentication
{
    public class JwtTokenConfig
    {
        public string Secret { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
    }
}
