using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

using FluentAssertions;

using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Jwt;
using ToDoList.WebApi.Jwt.Models;

using Xunit;

namespace WebApi.Tests.Jwt
{
    public class JwtTokenGeneratorTest
    {
        private readonly AuthenticationConfig authenticationConfig;
        private readonly JwtTokenGenerator jwtTokenGenerator;

        public JwtTokenGeneratorTest()
        {
            authenticationConfig = new AuthenticationConfig
            {
                Audience = "RandomAud",
                Issuer = "RandomIss",
                AccessTokenSecret = "ewowghewuoghefuqehfiqwuhfieqwofhqweoi",
                RefreshTokenSecret = "feifJP)F(JE-fj3fj-JFO3{dwfqwqdnafnow",
            };

            jwtTokenGenerator = new JwtTokenGenerator(authenticationConfig);
        }

        [Fact]
        public void GenerateAccessToken_ReturnsTokenGivenUser()
        {
            // Arrange
            UserResponse user = new(4, "John", "qwerty");

            // Act
            string token = jwtTokenGenerator.GenerateAccessToken(user);
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            // Assert
            jwtToken.Claims.Should().Contain(x => x.Value == user.Id.ToString());
            jwtToken.Claims.Should().Contain(x => x.Value == user.Name);

            jwtToken.Issuer.Should().Be(authenticationConfig.Issuer);
        }

        [Fact]
        public void GenerateRefreshToken_ReturnsToken()
        {
            // Act
            string token = jwtTokenGenerator.GenerateRefreshToken();
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            // Assert
            jwtToken.Issuer.Should().Be(authenticationConfig.Issuer);
        }
    }
}
