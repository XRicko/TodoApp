using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Moq;

using ToDoList.MvcClient.Controllers;
using ToDoList.SharedClientLibrary.Services;

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

            tokenStorageMock.Setup(x => x.GetTokenAsync("refreshToken"))
                            .ReturnsAsync("")
                            .Verifiable();

            // Act
            var result = await homeController.IndexAsync() as ViewResult;

            // Assert
            Assert.Equal(viewName, result.ViewName);
            tokenStorageMock.Verify();
        }

        [Fact]
        public async Task Index_ReturnsIndexViewIfTokenExists()
        {
            // Arrange
            string controllerName = "Todo";

            tokenStorageMock.Setup(x => x.GetTokenAsync("refreshToken"))
                            .ReturnsAsync("efjiJdjWFdojfw.PWQKDpfjejvnj")
                            .Verifiable();

            // Act
            var result = await homeController.IndexAsync() as RedirectToActionResult;

            // Assert
            Assert.Equal(controllerName, result.ControllerName);
            tokenStorageMock.Verify();
        }
    }
}
