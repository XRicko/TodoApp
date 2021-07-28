using System.Net.Http;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using Moq;

using ToDoList.MvcClient.Controllers;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Resources;

using Xunit;

namespace MvcClient.Tests.Controllers
{
    public class UserControllerTests : MvcControllerBaseForTests
    {
        private readonly Mock<IStringLocalizer<General>> localizerMock;
        private readonly UserController userController;

        private readonly string registerViewName;
        private readonly string loginViewName;

        private readonly string indexViewName;
        private readonly string todoControllerName;

        public UserControllerTests() : base()
        {
            localizerMock = new Mock<IStringLocalizer<General>>();
            userController = new UserController(ApiInvokerMock.Object, localizerMock.Object);

            registerViewName = "Register";
            loginViewName = "Login";

            indexViewName = "Index";
            todoControllerName = "Todo";
        }

        [Fact]
        public void Get_Login_ReturnsLoginView()
        {
            // Act
            var result = userController.Login() as ViewResult;

            // Assert
            result.ViewName.Should().Be(loginViewName);
        }

        [Fact]
        public void Get_Login_ReturnsRegisterView()
        {
            // Act
            var result = userController.Register() as ViewResult;

            // Assert
            result.ViewName.Should().Be(registerViewName);
        }

        [Fact]
        public async Task Post_Register_ReturnsSameViewGivenExistingUser()
        {
            // Arrange
            var existingUser = new UserModel { Name = "admin", Password = "qwerty", ConfirmPassword = "qwerty" };

            string key = "Registration Failed";
            string error = "User exists";

            var localizedString = new LocalizedString(key, error);


            ApiInvokerMock.Setup(x => x.AuthenticateUserAsync("Authentication/Register", existingUser))
                          .ThrowsAsync(new HttpRequestException("Error", null, System.Net.HttpStatusCode.Unauthorized))
                          .Verifiable();
            localizerMock.SetupGet(x => x[key])
                         .Returns(localizedString)
                         .Verifiable();

            // Act
            var result = await userController.RegisterAsync(existingUser) as ViewResult;

            // Assert
            result.ViewName.Should().Be(registerViewName);
            userController.ModelState[string.Empty].Errors.Should().Contain(x => x.ErrorMessage == error);

            ApiInvokerMock.Verify();
            localizerMock.Verify();
        }

        [Fact]
        public async Task Post_Register_ReturnsRedirectToIndexActionInTodoGivenNewUser()
        {
            // Arrange
            var newUser = new UserModel { Name = "admin", Password = "ytrewwq", ConfirmPassword = "ytrewq" };

            // Act 
            var result = await userController.RegisterAsync(newUser) as RedirectToActionResult;

            // Assert
            result.ActionName.Should().Be(indexViewName);
            result.ControllerName.Should().Be(todoControllerName);

            ApiInvokerMock.Verify(x => x.AuthenticateUserAsync("Authentication/Register", newUser), Times.Once);
        }

        [Fact]
        public async Task Post_Login_ReturnsSameViewGivenInvalidUser()
        {
            // Arrange
            var invalidUser = new UserModel { Name = "admin", Password = "invalid_password" };

            string key = "Login Failed";
            string error = "Username or password is incorrect";

            var localizedString = new LocalizedString(key, error);

            userController.ModelState.AddModelError("ConfirmPassword", "Confirm password doesnt match");

            ApiInvokerMock.Setup(x => x.AuthenticateUserAsync("Authentication/Login", invalidUser))
                          .ThrowsAsync(new HttpRequestException("Error", null, System.Net.HttpStatusCode.Unauthorized))
                          .Verifiable();
            localizerMock.SetupGet(x => x[key])
                         .Returns(localizedString)
                         .Verifiable();

            // Act
            var result = await userController.LoginAsync(invalidUser) as ViewResult;

            // Assert
            result.ViewName.Should().Be(loginViewName);

            userController.ModelState[string.Empty].Errors.Should().Contain(x => x.ErrorMessage == error);
            userController.ModelState["ConfirmPassword"].Errors.Should().BeEmpty();

            ApiInvokerMock.Verify();
            localizerMock.Verify();
        }

        [Fact]
        public async Task Post_Login_ReturnsRedirectToIndexActionInTodoGivenExistingUser()
        {
            // Arrange
            var existingUser = new UserModel { Name = "admin", Password = "qwerty" };

            // Act 
            var result = await userController.LoginAsync(existingUser) as RedirectToActionResult;

            // Assert
            result.ActionName.Should().Be(indexViewName);
            result.ControllerName.Should().Be(todoControllerName);

            ApiInvokerMock.Verify(x => x.AuthenticateUserAsync("Authentication/Login", existingUser), Times.Once);
        }

        [Fact]
        public async Task Get_Logout_ReturnsRedirectToIndexActionInHome()
        {
            // Arrange
            string homeControllerName = "Home";

            // Act 
            var result = await userController.LogoutAsync() as RedirectToActionResult;

            // Assert
            result.ActionName.Should().Be(indexViewName);
            result.ControllerName.Should().Be(homeControllerName);

            ApiInvokerMock.Verify(x => x.LogOutAsync(), Times.Once);
        }
    }
}
