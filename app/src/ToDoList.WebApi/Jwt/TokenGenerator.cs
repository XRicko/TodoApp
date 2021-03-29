using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

namespace ToDoList.WebApi.Jwt
{
<<<<<<< HEAD
    public class TokenGenerator : ITokenGenerator
=======
    internal class TokenGenerator : ITokenGenerator
>>>>>>> master
    {
        private readonly JwtTokenConfig jwtTokenConfig;

        public TokenGenerator(JwtTokenConfig tokenConfig)
        {
<<<<<<< HEAD
            jwtTokenConfig = tokenConfig;
=======
            jwtTokenConfig = tokenConfig ?? throw new ArgumentNullException(nameof(tokenConfig));
>>>>>>> master
        }

        public string GenerateToken(int id, string username)
        {
            var securityKey = jwtTokenConfig.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, id.ToString())
            };

            var token = new JwtSecurityToken(jwtTokenConfig.Issuer,
                                             jwtTokenConfig.Audience,
                                             claims,
                                             signingCredentials: credentials,
                                             expires: DateTime.Now.AddDays(1));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
