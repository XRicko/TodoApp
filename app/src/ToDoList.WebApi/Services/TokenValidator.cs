using System;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

using ToDoList.WebApi.Jwt.Models;

namespace ToDoList.WebApi.Services
{
    internal class TokenValidator : ITokenValidator
    {
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly AuthenticationConfig authenticationConfig;

        public TokenValidator(TokenValidationParameters validationParameters, AuthenticationConfig config)
        {
            tokenValidationParameters = validationParameters ?? throw new ArgumentNullException(nameof(validationParameters));
            authenticationConfig = config ?? throw new ArgumentNullException(nameof(config));
        }

        public bool ValidateRefreshKey(string refreshToken)
        {
            tokenValidationParameters.IssuerSigningKey = authenticationConfig.SymmetricSecurityRefreshKey;

            try
            {
                new JwtSecurityTokenHandler().ValidateToken(refreshToken, tokenValidationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
