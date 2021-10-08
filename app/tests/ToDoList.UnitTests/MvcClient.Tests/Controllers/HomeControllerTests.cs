using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using Moq;

using TestExtensions;

using ToDoList.MvcClient.Controllers;
using ToDoList.SharedClientLibrary.Services;
using ToDoList.SharedKernel;

using Xunit;

namespace MvcClient.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<ITokenStorage> tokenStorageMock;
        private readonly HomeController homeController;

        public HomeControllerTests()
        {
            tokenStorageMock = new Mock<ITokenStorage>();
            homeController = new HomeController(tokenStorageMock.Object);
        }

        [Fact]
        public async Task Index_ReturnsIndexViewIfNoToken()
        {
            // Arrange
            string viewName = "Index";

            tokenStorageMock.SetupGettingToken(Constants.RefreshToken, "");

            // Act
            var result = await homeController.IndexAsync() as ViewResult;

            // Assert
            result.ViewName.Should().Be(viewName);
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task Index_ReturnsIndexViewIfTokenExists()
        {
            // Arrange
            string controllerName = "Todo";

            tokenStorageMock.SetupGettingToken(Constants.RefreshToken, "efjiJdjWFdojfw.PWQKDpfjejvnj");

            // Act
            var result = await homeController.IndexAsync() as RedirectToActionResult;

            // Assert
            result.ControllerName.Should().Be(controllerName);
            tokenStorageMock.Verify();
        }
    }
}
