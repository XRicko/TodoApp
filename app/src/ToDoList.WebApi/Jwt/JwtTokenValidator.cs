using System;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

using ToDoList.WebApi.Jwt.Models;
using ToDoList.WebApi.Services;

namespace ToDoList.WebApi.Jwt
{
    public class JwtTokenValidator : ITokenValidator
    {
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly AuthenticationConfig authenticationConfig;

        public JwtTokenValidator(TokenValidationParameters validationParameters, AuthenticationConfig config)
        {
            tokenValidationParameters = validationParameters ?? throw new ArgumentNullException(nameof(validationParameters));
            authenticationConfig = config ?? throw new ArgumentNullException(nameof(config));
        }

        public bool ValidateRefreshKey(string refreshToken)
        {
            tokenValidationParameters.IssuerSigningKey = authenticationConfig.SymmetricSecurityRefreshKey;

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
