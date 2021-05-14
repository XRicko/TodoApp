using System;

using Microsoft.IdentityModel.Tokens;

using ToDoList.WebApi.Jwt;
using ToDoList.WebApi.Jwt.Models;

using Xunit;

namespace WebApi.Tests.Jwt
{
    public class JwtTokenValidatorTests
    {
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly AuthenticationConfig authenticationConfig;

        private readonly JwtTokenValidator jwtTokenValidator;

        public JwtTokenValidatorTests()
        {
            authenticationConfig = new AuthenticationConfig
            {
                Issuer = "RandIss",
                Audience = "SomeAud",
                AccessTokenSecret = "NOEFIfjewifpjwe)_Fef4ij",
                RefreshTokenSecret = "NFJOenfpf-4kp)KD#_(Jnjv",
                AccessTokenExpiryMinutes = 1,
                RefreshTokenExpiryMinutes = 1200
            };

            tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authenticationConfig.Issuer,

                ValidateAudience = true,
                ValidAudience = authenticationConfig.Audience,

                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                IssuerSigningKey = authenticationConfig.SymmetricSecurityAccessKey,
                ClockSkew = TimeSpan.Zero
            };

            jwtTokenValidator = new JwtTokenValidator(tokenValidationParameters, authenticationConfig);
        }

        [Fact]
        public void ValidateRefreshKey_ReturnsTrueGivenValidRefreshToken()
        {
            // Arrange
            string refreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2MjEwODEwMTYsImlzcyI6IlJhbmRJc3MiLCJhdWQiOiJTb21lQXVkIn0.U3VnaXZYMJoR6MTbA-Hb_RZdYwNM3lxwIk_Gk5q_XXw";

            // Act
            bool isValid = jwtTokenValidator.ValidateRefreshKey(refreshToken);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ValidateRefreshKey_ReturnsFalseGivenInvalidRefreshToken()
        {
            // Arrange
            string refreshToken = "eyJhbGciOiJIUzI1NyZInR5cCI6IkpXVCJ9.eyJleHAiOjE2MjEwODEwMTYsImlczyI6IlJhbmRJc3MiLCJhdWQiPiJTb21lQXVkIn0.U3VnaXZYMJoR6MTbA-Hb_RZdYwNM3lxwIk_Gk5q_XXw";

            // Act
            bool isValid = jwtTokenValidator.ValidateRefreshKey(refreshToken);

            // Assert
            Assert.False(isValid);
        }
    }
}
