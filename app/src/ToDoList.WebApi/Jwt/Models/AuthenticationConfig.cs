using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace ToDoList.WebApi.Jwt.Models
{
    [ExcludeFromCodeCoverage]
    public class AuthenticationConfig
    {
        [Required(AllowEmptyStrings = false)]
        public string Issuer { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Audience { get; set; }

        [Required]
        [MinLength(22)]
        public string AccessTokenSecret { get; set; }

        [Required]
        [MinLength(22)]
        public string RefreshTokenSecret { get; set; }

        [Required]
        public int AccessTokenExpiryMinutes { get; set; }

        [Required]
        public int RefreshTokenExpiryMinutes { get; set; }

        public SymmetricSecurityKey SymmetricSecurityAccessKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AccessTokenSecret));
        public SymmetricSecurityKey SymmetricSecurityRefreshKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RefreshTokenSecret));
    }
}
