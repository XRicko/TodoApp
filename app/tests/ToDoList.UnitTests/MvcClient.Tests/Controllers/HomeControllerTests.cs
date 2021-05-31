﻿using Microsoft.AspNetCore.Mvc;

using ToDoList.MvcClient.Controllers;

using Xunit;

namespace MvcClient.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly HomeController homeController;

        public HomeControllerTests()
        {
            homeController = new HomeController();
        }

        [Fact]
        public void ReturnsIndexView()
        {
            // Arrange
            string viewName = "Index";

            // Act
            var result = homeController.IndexAsync() as ViewResult;

            // Assert
            Assert.Equal(viewName, result.ViewName);
        }
    }
}
