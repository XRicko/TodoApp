using System;
using System.Security.Claims;

using ToDoList.SharedClientLibrary.Services;

using Xunit;

namespace SharedClientLibrary.Tests.Services
{
    public class JwtTokenParserTests
    {
        private readonly JwtTokenParser jwtTokenParser;

        private readonly string token;
        private readonly DateTimeOffset expiryDate;

        public JwtTokenParserTests()
        {
            jwtTokenParser = new JwtTokenParser();

            token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJSYW5kSXNzIiwiaWF0IjoxNjIxMzQ4MTc2LCJleHAiOjE2NTI4ODQxNzYsImF1ZCI6IlNvbWVBdWQiLCJzdWIiOiJqcm9ja2V0QGV4YW1wbGUuY29tIn0.DnSrB6eJGalklDvLZSQzpx-_jQnP3-SiTWNweRZwPq0";
            expiryDate = new DateTimeOffset(2022, 05, 18, 14, 29, 36, TimeSpan.Zero);
        }

        [Fact]
        public void GetExpiryDate_ReturnsExpiryDateGivenToken()
        {
            // Act
            var actual = jwtTokenParser.GetExpiryDate(token);

            // Assert
            Assert.Equal(expiryDate, actual);
        }

        [Fact]
        public void GetExpiryDate_ReturnsExpiryDateGivenTokenAndClaimsPrincipal()
        {
            // Arrange
            ClaimsPrincipal claimsPrincipal = new(new ClaimsIdentity(new Claim[]
            {
                new Claim("exp", expiryDate.ToUnixTimeSeconds().ToString())
            }));

            // Act
            var actual = jwtTokenParser.GetExpiryDate(token, claimsPrincipal);

            // Assert
            Assert.Equal(expiryDate, actual);
        }

        [Fact]
        public void GetClaimsPrincipal_ReturnsClaimsIdentityGivenToken()
        {
            // Act
            var actual = jwtTokenParser.GetClaimsPrincipal(token);

            // Assert
            Assert.Equal(expiryDate.ToUnixTimeSeconds().ToString(), actual.FindFirst("exp").Value);
        }
    }
}
