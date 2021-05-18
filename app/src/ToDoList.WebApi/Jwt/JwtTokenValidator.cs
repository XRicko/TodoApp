using System;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

using ToDoList.WebApi.Jwt.Models;
using ToDoList.WebApi.Services;

namespace ToDoList.WebApi.Jwt
{
    public class JwtTokenValidator : ITokenValidator
    {
        private readonly AuthenticationConfig authenticationConfig;

        public JwtTokenValidator(AuthenticationConfig config)
        {
            authenticationConfig = config ?? throw new ArgumentNullException(nameof(config));
        }

        public bool ValidateRefreshKey(string refreshToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authenticationConfig.Issuer,

                ValidateAudience = true,
                ValidAudience = authenticationConfig.Audience,

                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                IssuerSigningKey = authenticationConfig.SymmetricSecurityRefreshKey,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                new JwtSecurityTokenHandler().ValidateToken(refreshToken, tokenValidationParameters, out var validatedToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
