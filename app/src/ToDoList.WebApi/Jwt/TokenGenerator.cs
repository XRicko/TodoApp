using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Jwt.Models;

namespace ToDoList.WebApi.Jwt
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly AuthenticationConfig authenticationConfig;

        public TokenGenerator(AuthenticationConfig config)
        {
            authenticationConfig = config ?? throw new ArgumentNullException(nameof(config));
        }

        public string GenerateAccessToken(UserResponse user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            return GenerateToken(authenticationConfig.AccessTokenExpiryMinutes,
                                 authenticationConfig.SymmetricSecurityAccessKey,
                                 claims);
        }

        public string GenerateRefreshToken()
        {
            return GenerateToken(authenticationConfig.RefreshTokenExpiryMinutes,
                                 authenticationConfig.SymmetricSecurityRefreshKey);
        }

        private string GenerateToken(int expiryMinutes, SymmetricSecurityKey securityKey, IEnumerable<Claim> claims = null)
        {
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(authenticationConfig.Issuer,
                                             authenticationConfig.Audience,
                                             claims,
                                             signingCredentials: credentials,
                                             expires: DateTime.Now.AddMinutes(expiryMinutes));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
