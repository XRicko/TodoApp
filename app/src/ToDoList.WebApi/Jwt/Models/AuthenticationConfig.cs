using System.ComponentModel.DataAnnotations;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace ToDoList.WebApi.Jwt.Models
{
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

        public SymmetricSecurityKey SymmetricSecurityAccessKey => new(Encoding.UTF8.GetBytes(AccessTokenSecret));
        public SymmetricSecurityKey SymmetricSecurityRefreshKey => new(Encoding.UTF8.GetBytes(RefreshTokenSecret));
    }
}
