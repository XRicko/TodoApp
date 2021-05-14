using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Commands.RefreshTokens;
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
        public async Task Login_ReturnsOkResultWithAuthenticatedResponseGivenExistingUser()
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
            var actionResult = await authenticationController.LoginAsync(userRequest);
            var okResult = actionResult as OkObjectResult;

            var actual = okResult.Value as AuthenticatedResponse;

            // Assert
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expected, actual);

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
            var actionResult = await authenticationController.LoginAsync(userRequest);
            var unauthorizedResult = actionResult as UnauthorizedObjectResult;

            string actual = unauthorizedResult.Value as string;

            // Assert
            Assert.Equal(401, unauthorizedResult.StatusCode);
            Assert.Equal(expectedMessage, actual);

            MediatorMock.Verify();
            authenticatorMock.Verify(x => x.AuthenticateAsync(It.IsAny<UserResponse>()), Times.Never);
        }

        [Fact]
        public async Task Register_ReturnsOkResultWithAuthenticatedResponseGivenNewUser()
        {
            // Arrange
            var userResponse = new UserResponse(3, "admin", "password");

            string accessToken = "eUnfjsnaqqopd_sOPmNaj";
            string refreshToken = "J|EVdAy60Fk9QZaShYUFE6";

            AuthenticatedResponse expected = new(accessToken, refreshToken);

            MediatorMock.SetupSequence(x => x.Send(new GetUserByNameAndPasswordQuery(userRequest.Name, userRequest.Password),
                                                   It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => null)
                        .ReturnsAsync(userResponse);

            authenticatorMock.Setup(x => x.AuthenticateAsync(userResponse))
                             .ReturnsAsync(expected)
                             .Verifiable();

            // Act
            var actionResult = await authenticationController.RegisterAsync(userRequest);
            var okResult = actionResult as OkObjectResult;

            var actual = okResult.Value as AuthenticatedResponse;

            // Assert
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expected, actual);

            MediatorMock.Verify(x => x.Send(new GetUserByNameAndPasswordQuery(userRequest.Name, userRequest.Password),
                                            It.IsAny<CancellationToken>()), Times.Exactly(2));
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

            MediatorMock.Setup(x => x.Send(new GetUserByNameAndPasswordQuery(userRequest.Name, userRequest.Password),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(userResponse)
                        .Verifiable();

            // Act
            var actionResult = await authenticationController.RegisterAsync(userRequest);
            var unauthorizedResult = actionResult as UnauthorizedObjectResult;

            string actual = unauthorizedResult.Value as string;

            // Assert
            Assert.Equal(401, unauthorizedResult.StatusCode);
            Assert.Equal(expectedMessage, actual);

            MediatorMock.Verify();
            MediatorMock.Verify(x => x.Send(new AddCommand<UserRequest>(userRequest),
                                            It.IsAny<CancellationToken>()), Times.Never);

            authenticatorMock.Verify(x => x.AuthenticateAsync(It.IsAny<UserResponse>()), Times.Never);
        }

        [Fact]
        public async Task Logout_ReturnsOkResultOnSuccess()
        {
            // Arrange
            int userId = 13;

            var contextMock = new Mock<HttpContext>();
            authenticationController.ControllerContext.HttpContext = contextMock.Object;

            var claim = new Claim(ClaimTypes.NameIdentifier, userId.ToString());

            contextMock.Setup(x => x.User.FindFirst(ClaimTypes.NameIdentifier))
                       .Returns(claim);

            // Act
            var actionResult = await authenticationController.Logout();
            var okResult = actionResult as OkResult;

            // Assert
            Assert.Equal(200, okResult.StatusCode);
            MediatorMock.Verify(x => x.Send(new RemoveAllRefreshTokensFromUserCommand(userId),
                                            It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Logut_ReturnsUnauthorizedWhenUserIdNull()
        {
            // Act
            var actionResult = await authenticationController.Logout();
            var unauthorizedResult = actionResult as UnauthorizedResult;

            // Assert
            Assert.Equal(401, unauthorizedResult.StatusCode);
            MediatorMock.Verify(x => x.Send(new RemoveAllRefreshTokensFromUserCommand(It.IsAny<int>()),
                                            It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
