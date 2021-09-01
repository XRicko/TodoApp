
using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ToDoList.MvcClient.Controllers;

using Xunit;

namespace MvcClient.Tests.Controllers
{
    public class LocalizationControllerTests
    {
        private readonly LocalizationController controller;

        public LocalizationControllerTests()
        {
            controller = new LocalizationController();

            var context = new DefaultHttpContext();
            controller.ControllerContext.HttpContext = context;
        }

        [Fact]
        public void SetLanguage()
        {
            // Arrange
            string url = "https://localhost//something";
            string culture = "uk";

            // Act
            var result = controller.SetLanguage(culture, url) as LocalRedirectResult;

            // Assert
            result.Url.Should().Be(url);
        }
    }
}
