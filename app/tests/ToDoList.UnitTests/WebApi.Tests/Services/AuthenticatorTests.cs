using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Moq;

using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Jwt.Models;
using ToDoList.WebApi.Services;

using Xunit;

namespace WebApi.Tests.Services
{
    public class AuthenticatorTests
    {
        private readonly Mock<ITokenGenerator> tokenGeneratorMock;
        private readonly Mock<IMediator> mediatorMock;

        private readonly Authenticator authenticator;

        public AuthenticatorTests()
        {
            tokenGeneratorMock = new Mock<ITokenGenerator>();
            mediatorMock = new Mock<IMediator>();

            authenticator = new Authenticator(tokenGeneratorMock.Object, mediatorMock.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsAuthenticatedResponseGivenUser()
        {
            // Arrange
            UserResponse user = new(12, "admin", "qwerty");

            string accessToken = "eUnfjsnaqqopd_sOPmNaj";
            string refreshToken = "J|EVdAy60Fk9QZaShYUFE6";

            AuthenticatedResponse expected = new(accessToken, refreshToken);

            tokenGeneratorMock.Setup(x => x.GenerateAccessToken(user))
                              .Returns(accessToken)
                              .Verifiable();

            tokenGeneratorMock.Setup(x => x.GenerateRefreshToken())
                              .Returns(refreshToken)
                              .Verifiable();

            // Act
            var actual = await authenticator.AuthenticateAsync(user);

            // Assert
            Assert.Equal(expected, actual);

            tokenGeneratorMock.Verify();
            mediatorMock.Verify(x => x.Send(It.Is<AddCommand<RefreshTokenCreateRequest>>(q => q.Request.Name == refreshToken
                                                                                              && q.Request.UserId == user.Id),
                                            It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
