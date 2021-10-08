using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Commands.RefreshTokens;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Queries.RefreshTokens;
using ToDoList.Core.Mediator.Queries.Users;
using ToDoList.Core.Mediator.Requests;
using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Controllers;
using ToDoList.WebApi.Jwt.Models;
using ToDoList.WebApi.Services;

using Xunit;

namespace WebApi.Tests.Controllers
{
    public class AuthenticationControllerTests : ApiControllerBaseForTests
    {
        private readonly Mock<IAuthenticator> authenticatorMock;
        private readonly Mock<ITokenValidator> tokenValidatorMock;

        private readonly AuthenticationController authenticationController;

        private readonly UserRequest userRequest;

        public AuthenticationControllerTests() : base()
        {
            authenticatorMock = new Mock<IAuthenticator>();
            tokenValidatorMock = new Mock<ITokenValidator>();

            authenticationController = new AuthenticationController(MediatorMock.Object, authenticatorMock.Object, tokenValidatorMock.Object);

            userRequest = new UserRequest("admin", "password");
        }

        [Fact]
        public async Task Login_ReturnsAuthenticatedResponseGivenExistingUser()
        {
            // Arrange
            var userResponse = new UserResponse(3, "admin", "password");

            string accessToken = "eUnfjsnaqqopd_sOPmNaj";
            string refreshToken = "J|EVdAy60Fk9QZaShYUFE6";

            AuthenticatedResponse expected = new(accessToken, refreshToken);

            MediatorMock.Setup(x => x.Send(new GetUserByNameAndPasswordQuery(userRequest.Name, userRequest.Password),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(userResponse)
                        .Verifiable();

            authenticatorMock.Setup(x => x.AuthenticateAsync(userResponse))
                             .ReturnsAsync(expected)
                             .Verifiable();

            // Act
            var actual = await authenticationController.LoginAsync(userRequest); ;

            // Assert
            actual.Value.Should().Be(expected);

            MediatorMock.Verify();
            authenticatorMock.Verify();
        }

        [Fact]
        public async Task Login_ReturnsUnauthorizedGivenNotRegisteredUser()
        {
            // Arrange
            string expectedMessage = "Username or password is incorrect";

            MediatorMock.Setup(x => x.Send(new GetUserByNameAndPasswordQuery(userRequest.Name, userRequest.Password),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => null)
                        .Verifiable();

            // Act
            var result = await authenticationController.LoginAsync(userRequest);
            var unauthorizedObjectResult = result.Result as UnauthorizedObjectResult;

            // Assert
            unauthorizedObjectResult.Should().NotBeNull();
            unauthorizedObjectResult.Value.Should().BeEquivalentTo(expectedMessage);

            MediatorMock.Verify();
            authenticatorMock.Verify(x => x.AuthenticateAsync(It.IsAny<UserResponse>()), Times.Never);
        }

        [Fact]
        public async Task Register_ReturnsAuthenticatedResponseGivenNewUser()
        {
            // Arrange
            var userResponse = new UserResponse(3, "admin", "password");

            string accessToken = "eUnfjsnaqqopd_sOPmNaj";
            string refreshToken = "J|EVdAy60Fk9QZaShYUFE6";

            AuthenticatedResponse expected = new(accessToken, refreshToken);

            MediatorMock.Setup(x => x.Send(new GetByNameQuery<User, UserResponse>(userRequest.Name),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => null)
                        .Verifiable();

            MediatorMock.Setup(x => x.Send(new GetUserByNameAndPasswordQuery(userRequest.Name, userRequest.Password),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(userResponse)
                        .Verifiable();

            authenticatorMock.Setup(x => x.AuthenticateAsync(userResponse))
                             .ReturnsAsync(expected)
                             .Verifiable();

            // Act
            var actual = await authenticationController.RegisterAsync(userRequest);

            // Assert
            actual.Value.Should().Be(expected);

            MediatorMock.Verify();
            MediatorMock.Verify(x => x.Send(new AddCommand<UserRequest>(userRequest),
                                            It.IsAny<CancellationToken>()), Times.Once);

            authenticatorMock.Verify();
        }

        [Fact]
        public async Task Register_ReturnsBadRequestGivenExistingUser()
        {
            // Arrange
            string expectedMessage = "User exists";
            var userResponse = new UserResponse(3, "admin", "password");

            MediatorMock.Setup(x => x.Send(new GetByNameQuery<User, UserResponse>(userRequest.Name),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(userResponse)
                        .Verifiable();

            // Act
            var actionResult = await authenticationController.RegisterAsync(userRequest);
            var unauthorizedResult = actionResult.Result as UnauthorizedObjectResult;

            // Assert
            unauthorizedResult.Should().NotBeNull();
            unauthorizedResult.Value.Should().BeEquivalentTo(expectedMessage);

            MediatorMock.Verify();
            MediatorMock.Verify(x => x.Send(new AddCommand<UserRequest>(userRequest),
                                            It.IsAny<CancellationToken>()), Times.Never);

            authenticatorMock.Verify(x => x.AuthenticateAsync(It.IsAny<UserResponse>()), Times.Never);
        }

        [Fact]
        public async Task LogoutEverywhere_ReturnsOkResultOnSuccess()
        {
            // Arrange
            int userId = 13;

            var contextMock = new Mock<HttpContext>();
            authenticationController.ControllerContext.HttpContext = contextMock.Object;

            var claim = new Claim(ClaimTypes.NameIdentifier, userId.ToString());

            contextMock.Setup(x => x.User.FindFirst(ClaimTypes.NameIdentifier))
                       .Returns(claim)
                       .Verifiable();

            // Act
            var actionResult = await authenticationController.LogoutEverywhere();

            // Assert
            actionResult.Should().BeAssignableTo<OkResult>();

            MediatorMock.Verify(x => x.Send(new RemoveAllRefreshTokensFromUserCommand(userId),
                                            It.IsAny<CancellationToken>()), Times.Once);
            contextMock.Verify();
        }

        [Fact]
        public async Task LogoutEverywhere_ReturnsUnauthorizedWhenUserIdNull()
        {
            // Act
            var actionResult = await authenticationController.LogoutEverywhere();

            // Assert
            actionResult.Should().BeAssignableTo<UnauthorizedResult>();
            MediatorMock.Verify(x => x.Send(new RemoveAllRefreshTokensFromUserCommand(It.IsAny<int>()),
                                            It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Logout_ReturnsOkResult()
        {
            // Arrange
            string refreshToken = "EyJdnjEWFO.QnjQvnXJ";

            // Act
            var actionResult = await authenticationController.Logout(refreshToken);

            // Assert
            actionResult.Should().BeAssignableTo<OkResult>();
            MediatorMock.Verify(x => x.Send(new RemoveByNameCommand<RefreshToken>(refreshToken), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Refresh_ReturnsUnauthorizedWhenInvalidRefreshToken()
        {
            // Arrange
            string refreshToken = "efjieofnsXqweHEq.Qjsnzeqfi";
            string message = "Invalid refresh token";

            tokenValidatorMock.Setup(x => x.ValidateRefreshToken(refreshToken))
                              .Returns(false)
                              .Verifiable();

            // Act
            var result = await authenticationController.RefreshAsync(refreshToken);
            var unauthorizedObjectResult = result.Result as UnauthorizedObjectResult;

            // Assert
            unauthorizedObjectResult.Should().NotBeNull();
            unauthorizedObjectResult.Value.Should().BeEquivalentTo(message);

            tokenValidatorMock.Verify();
        }

        [Fact]
        public async Task Refresh_ReturnsUnauthorizedWhenNoRefreshToken()
        {
            // Arrange
            string refreshToken = "efjieofnsXqweHEq.Qjsnzeqfi";
            string message = "No refresh token";

            tokenValidatorMock.Setup(x => x.ValidateRefreshToken(refreshToken))
                              .Returns(true)
                              .Verifiable();

            MediatorMock.Setup(x => x.Send(new GetRefreshTokenByTokenQuery(refreshToken), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => null)
                        .Verifiable();

            // Act
            var result = await authenticationController.RefreshAsync(refreshToken);
            var unauthorizedObjectResult = result.Result as UnauthorizedObjectResult;

            // Assert
            unauthorizedObjectResult.Should().NotBeNull();
            unauthorizedObjectResult.Value.Should().BeEquivalentTo(message);

            tokenValidatorMock.Verify();
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Refresh_ReturnsUnauthorizedWhenNoUser()
        {
            // Arrange
            string refreshToken = "efjieofnsXqweHEq.Qjsnzeqfi";
            string message = "User not found";

            RefreshTokenResponse refreshTokenResponse = new(31, refreshToken, 5);

            tokenValidatorMock.Setup(x => x.ValidateRefreshToken(refreshToken))
                              .Returns(true)
                              .Verifiable();

            MediatorMock.Setup(x => x.Send(new GetRefreshTokenByTokenQuery(refreshToken), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(refreshTokenResponse)
                        .Verifiable();

            MediatorMock.Setup(x => x.Send(new RemoveByNameCommand<RefreshToken>(refreshTokenResponse.Name), It.IsAny<CancellationToken>()))
                        .Verifiable();

            MediatorMock.Setup(x => x.Send(new GetByIdQuery<User, UserResponse>(refreshTokenResponse.UserId),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => null)
                        .Verifiable();

            // Act
            var result = await authenticationController.RefreshAsync(refreshToken);
            var unauthorizedObjectResult = result.Result as UnauthorizedObjectResult;

            // Assert
            unauthorizedObjectResult.Should().NotBeNull();
            unauthorizedObjectResult.Value.Should().BeEquivalentTo(message);

            tokenValidatorMock.Verify();
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Refresh_ReturnsAuthenticatedResponse()
        {
            // Arrange
            string accessToken = "eUnfjsnaqqopd_sOPmNaj";
            string refreshToken = "efjieofnsXqweHEq.Qjsnzeqfi";

            UserResponse userResponse = new(5, "admin", "qwerty");
            RefreshTokenResponse refreshTokenResponse = new(31, refreshToken, userResponse.Id);

            AuthenticatedResponse expected = new(accessToken, refreshToken);

            tokenValidatorMock.Setup(x => x.ValidateRefreshToken(refreshToken))
                              .Returns(true)
                              .Verifiable();

            MediatorMock.Setup(x => x.Send(new GetRefreshTokenByTokenQuery(refreshToken), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(refreshTokenResponse)
                        .Verifiable();

            MediatorMock.Setup(x => x.Send(new RemoveByNameCommand<RefreshToken>(refreshTokenResponse.Name), It.IsAny<CancellationToken>()))
                        .Verifiable();

            MediatorMock.Setup(x => x.Send(new GetByIdQuery<User, UserResponse>(refreshTokenResponse.UserId),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(userResponse)
                        .Verifiable();

            authenticatorMock.Setup(x => x.AuthenticateAsync(userResponse))
                             .ReturnsAsync(expected)
                             .Verifiable();

            // Act
            var result = await authenticationController.RefreshAsync(refreshToken);

            // Assert
            result.Value.Should().Be(expected);

            tokenValidatorMock.Verify();
            MediatorMock.Verify();
            authenticatorMock.Verify();
        }
    }
}
