
using FluentAssertions;

using ToDoList.WebApi.Jwt;
using ToDoList.WebApi.Jwt.Models;

using Xunit;

namespace WebApi.Tests.Jwt
{
    public class JwtTokenValidatorTests
    {
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
                RefreshTokenExpiryMinutes = 131400
            };

            jwtTokenValidator = new JwtTokenValidator(authenticationConfig);
        }

        [Fact]
        public void ValidateRefreshKey_ReturnsTrueGivenValidRefreshToken()
        {
            // Arrange
            string refreshToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJSYW5kSXNzIiwiaWF0IjoxNjIxMzQ4MTc2LCJleHAiOjE2NTI4ODQxNzYsImF1ZCI6IlNvbWVBdWQiLCJzdWIiOiJqcm9ja2V0QGV4YW1wbGUuY29tIn0.DnSrB6eJGalklDvLZSQzpx-_jQnP3-SiTWNweRZwPq0";

            // Act
            bool isValid = jwtTokenValidator.ValidateRefreshKey(refreshToken);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void ValidateRefreshKey_ReturnsFalseGivenInvalidRefreshToken()
        {
            // Arrange
            string refreshToken = "eyJhbGciOiJIUzI1NyZInR5cCI6IkpXVCJ9.eyJleHAiOjE2MjEwODEwMTYsImlczyI6IlJhbmRJc3MiLCJhdWQiPiJTb21lQXVkIn0.U3VnaXZYMJoR6MTbA-Hb_RZdYwNM3lxwIk_Gk5q_XXw";

            // Act
            bool isValid = jwtTokenValidator.ValidateRefreshKey(refreshToken);

            // Assert
            isValid.Should().BeFalse();
        }
    }
}
