using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using Moq;

using ToDoList.MvcClient.Controllers;
using ToDoList.MvcClient.Models;

using Xunit;

namespace MvcClient.Tests.Controllers
{
    public class UserControllerTests : MvcControllerBaseForTests
    {
        private readonly Mock<IStringLocalizer<UserController>> localizerMock;
        private readonly UserController userController;

        private readonly string registerViewName;
        private readonly string loginViewName;

        private readonly string indexViewName;
        private readonly string todoControllerName;

        public UserControllerTests() : base()
        {
            localizerMock = new Mock<IStringLocalizer<UserController>>();
            userController = new UserController(ApiCallsServiceMock.Object, localizerMock.Object);

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
            Assert.Equal(loginViewName, result.ViewName);
        }

        [Fact]
        public void Get_Login_ReturnsRegisterView()
        {
            // Act
            var result = userController.Register() as ViewResult;

            // Assert
            Assert.Equal(registerViewName, result.ViewName);
        }

        [Fact]
        public async Task Post_Register_ReturnsSameViewGivenExistingUser()
        {
            // Arrange
            var existingUser = new UserModel { Name = "admin", Password = "qwerty", ConfirmPassword = "qwerty" };

            string error = "User exists";
            string key = "UserExists";
            var localizedString = new LocalizedString(key, error);


            ApiCallsServiceMock.Setup(x => x.AuthenticateUserAsync("User/Register", existingUser))
                               .ThrowsAsync(new Exception("Unauthorized"))
                               .Verifiable();
            localizerMock.SetupGet(x => x[key])
                         .Returns(localizedString)
                         .Verifiable();

            // Act
            var result = await userController.RegisterAsync(existingUser) as ViewResult;

            // Assert
            Assert.Equal(registerViewName, result.ViewName);
            Assert.Contains(userController.ModelState[string.Empty].Errors, e => e.ErrorMessage == error);

            ApiCallsServiceMock.Verify();
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
            Assert.Equal(indexViewName, result.ActionName);
            Assert.Equal(todoControllerName, result.ControllerName);

            ApiCallsServiceMock.Verify(x => x.AuthenticateUserAsync("User/Register", newUser), Times.Once);
        }

        [Fact]
        public async Task Post_Login_ReturnsSameViewGivenInvalidUser()
        {
            // Arrange
            var invalidUser = new UserModel { Name = "admin", Password = "invalid_password" };

            string key = "InvalidCredentials";
            string error = "Username or password is incorrect";
            var localizedString = new LocalizedString(key, error);

            userController.ModelState.AddModelError("ConfirmPassword", "Confirm password doesnt match");

            ApiCallsServiceMock.Setup(x => x.AuthenticateUserAsync("User/Login", invalidUser))
                               .ThrowsAsync(new Exception("Unauthorized"))
                               .Verifiable();
            localizerMock.SetupGet(x => x[key])
                         .Returns(localizedString)
                         .Verifiable();

            // Act
            var result = await userController.LoginAsync(invalidUser) as ViewResult;

            // Assert
            Assert.Equal(loginViewName, result.ViewName);
            Assert.Contains(userController.ModelState[string.Empty].Errors, e => e.ErrorMessage == error);
            Assert.Empty(userController.ModelState["ConfirmPassword"].Errors);

            ApiCallsServiceMock.Verify();
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
            Assert.Equal(indexViewName, result.ActionName);
            Assert.Equal(todoControllerName, result.ControllerName);

            ApiCallsServiceMock.Verify(x => x.AuthenticateUserAsync("User/Login", existingUser), Times.Once);
        }

        [Fact]
        public void Get_Logout_ReturnsRedirectToIndexActionInHome()
        {
            // Arrange
            Mock<ControllerContext> controllerContextMock = new();
            Mock<HttpContext> httpContextMock = new();

            httpContextMock.Setup(x => x.Response.Cookies.Delete("Token"))
                           .Verifiable();
            controllerContextMock.Object.HttpContext = httpContextMock.Object;

            userController.ControllerContext = controllerContextMock.Object;

            string homeControllerName = "Home";

            // Act 
            var result = userController.Logout() as RedirectToActionResult;

            // Assert
            Assert.Equal(indexViewName, result.ActionName);
            Assert.Equal(homeControllerName, result.ControllerName);

            httpContextMock.Verify();
        }
    }
}
