using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Jwt;
using ToDoList.WebApi.Jwt.Models;

using Xunit;

namespace ToDoList.UnitTests.WebApi.Jwt
{
    public class TokenGeneratorTest
    {
        private readonly AuthenticationConfig jwtTokenConfig;
        private readonly TokenGenerator tokenGenerator;

        public TokenGeneratorTest()
        {
            jwtTokenConfig = new AuthenticationConfig
            {
                Audience = "RandomAud",
                Issuer = "RandomIss",
                AccessTokenSecret = "ewowghewuoghefuqehfiqwuhfieqwofhqweoi"
            };

            tokenGenerator = new TokenGenerator(jwtTokenConfig);
        }

        [Fact]
        public void GenerateToken_ReturnsTokenGivenIdAndName()
        {
            // Arrange
            UserResponse user = new(4, "John", "qwerty");

            // Act
            string token = tokenGenerator.GenerateAccessToken(user);
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            // Assert
            Assert.Equal(user.Id.ToString(), jwtToken.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            Assert.Equal(user.Name, jwtToken.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Name).Value);
        }
    }
}
