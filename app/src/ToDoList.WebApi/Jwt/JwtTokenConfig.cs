using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace ToDoList.WebApi.Jwt
{
    [ExcludeFromCodeCoverage]
    public class JwtTokenConfig
    {
        [Required(AllowEmptyStrings = false)]
        public string Issuer { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Audience { get; set; }

        [Required]
        [MinLength(22)]
        public string Secret { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
    }
}
