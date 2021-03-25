using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Moq;

using ToDoList.Core.Mediator.Queries.Users;
using ToDoList.Core.Mediator.Requests;
using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Controllers;
using ToDoList.WebApi.Jwt;

using Xunit;

namespace ToDoList.UnitTests.WebApi.Controllers
{
    public class UserControllerTests : ControllerBaseForTests
    {
        private readonly UserController userController;
        private readonly Mock<ITokenGenerator> tockenGeneratorMock;

        private readonly UserRequest userRequest;

        public UserControllerTests() : base()
        {
            tockenGeneratorMock = new Mock<ITokenGenerator>();
            userController = new UserController(MediatorMock.Object, tockenGeneratorMock.Object);

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
                        .ReturnsAsync(userResponse);

            tockenGeneratorMock.Setup(x => x.GenerateToken(userResponse.Id, userResponse.Name))
                               .Returns(expected);
            // Act
            var actionResult = await userController.LoginAsync(userRequest);
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
            string expectedMessage = "No user found";

            MediatorMock.Setup(x => x.Send(It.Is<GetUserByNameAndPasswordQuery>(q => q.Name == userRequest.Name
                                                                                     && q.Password == userRequest.Password),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => null);

            // Act
            var actionResult = await userController.LoginAsync(userRequest);
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

            tockenGeneratorMock.Setup(x => x.GenerateToken(userResponse.Id, userResponse.Name))
                               .Returns(expected);

            // Act
            var actionResult = await userController.RegisterAsync(userRequest);
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

            MediatorMock.Setup(x => x.Send(It.Is<GetUserByNameAndPasswordQuery>(q => q.Name == userRequest.Name
                                                                                     && q.Password == userRequest.Password),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(userResponse);

            // Act
            var actionResult = await userController.RegisterAsync(userRequest);
            var badRequestResult = actionResult as BadRequestObjectResult;

            string actual = badRequestResult.Value as string;

            // Assert
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal(expectedMessage, actual);

            MediatorMock.Verify();
        }
    }
}
