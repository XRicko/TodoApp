using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

using ToDoList.WebApi.Jwt;

using Xunit;

namespace ToDoList.UnitTests.WebApi.Jwt
{
    public class TokenGeneratorTest
    {
        private readonly JwtTokenConfig jwtTokenConfig;
        private readonly TokenGenerator tokenGenerator;

        public TokenGeneratorTest()
        {
            jwtTokenConfig = new JwtTokenConfig { Audience = "RandomAud", Issuer = "RandomIss", Secret = "ewowghewuoghefuqehfiqwuhfieqwofhqweoi" };
            tokenGenerator = new TokenGenerator(jwtTokenConfig);
        }

        [Fact]
        public void GenerateToken_ReturnsTokenGivenIdAndName()
        {
            // Arrange
            int id = 4;
            string name = "John";

            // Act
            string token = tokenGenerator.GenerateToken(id, name);
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            // Assert
            Assert.Equal(id.ToString(), jwtToken.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            Assert.Equal(name, jwtToken.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Name).Value);
        }
    }
}
