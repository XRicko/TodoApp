using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Moq;

using ToDoList.Core.Mediator.Queries.Users;
using ToDoList.Core.Mediator.Requests;
using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Controllers;
using ToDoList.WebApi.Jwt;
using ToDoList.WebApi.Services;

using Xunit;

namespace ToDoList.UnitTests.WebApi.Controllers
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
        public async Task Login_ReturnsOkResultWithTokenGivenExistingUser()
        {
            // Arrange
            var userResponse = new UserResponse(3, "admin", "password");

            string expected = "eUnfjsnaqqopd_sOPmNaj";

            MediatorMock.Setup(x => x.Send(It.Is<GetUserByNameAndPasswordQuery>(q => q.Name == userRequest.Name
                                                                                     && q.Password == userRequest.Password), 
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(userResponse)
                        .Verifiable();

            tockenGeneratorMock.Setup(x => x.GenerateAccessToken(userResponse))
                               .Returns(expected)
                               .Verifiable();
            // Act
            var actionResult = await authenticationController.LoginAsync(userRequest);
            var okResult = actionResult as OkObjectResult;

            string actual = okResult.Value as string;

            // Assert
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expected, actual);

            MediatorMock.Verify();
            tockenGeneratorMock.Verify();
        }

        [Fact]
        public async Task Login_ReturnsUnauthorizedGivenNotRegisteredUser()
        {
            // Arrange
            string expectedMessage = "Username or password is incorrect";

            MediatorMock.Setup(x => x.Send(It.Is<GetUserByNameAndPasswordQuery>(q => q.Name == userRequest.Name
                                                                                     && q.Password == userRequest.Password), It.IsAny<CancellationToken>()))
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
        }

        [Fact]
        public async Task Register_ReturnsOkResultWithTokenGivenNewUser()
        {
            // Arrange
            var userResponse = new UserResponse(3, "admin", "password");

            string expected = "eUnfjsnaqqopd_sOPmNaj";

            MediatorMock.SetupSequence(x => x.Send(It.Is<GetUserByNameAndPasswordQuery>(q => q.Name == userRequest.Name
                                                                                             && q.Password == userRequest.Password),
                                                   It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => null)
                        .ReturnsAsync(userResponse);

            tockenGeneratorMock.Setup(x => x.GenerateAccessToken(userResponse))
                               .Returns(expected)
                               .Verifiable();

            // Act
            var actionResult = await authenticationController.RegisterAsync(userRequest);
            var okResult = actionResult as OkObjectResult;

            string actual = okResult.Value as string;

            // Assert
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expected, actual);

            MediatorMock.Verify();
            tockenGeneratorMock.Verify();
        }

        [Fact]
        public async Task Register_ReturnsBadRequestGivenExistingUser()
        {
            // Arrange
            string expectedMessage = "User exists";
            var userResponse = new UserResponse(3, "admin", "password");

            MediatorMock.Setup(x => x.Send(It.Is<GetUserByNameAndPasswordQuery>(q => q.Name == userRequest.Name && q.Password == userRequest.Password), It.IsAny<CancellationToken>()))
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
        }
    }
}
