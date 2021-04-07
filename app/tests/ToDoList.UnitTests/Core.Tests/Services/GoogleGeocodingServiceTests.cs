using System;

using ToDoList.Core;
using ToDoList.Core.Services;

using Xunit;

namespace Core.Tests.Services
{
    public class GoogleGeocodingServiceTests
    {
        [Fact]
        public void Constructor_ThrowsArgumentNullExceptionGivenNullApiOptions()
        {
            // Arrange
            ApiOptions apiOptions = null;

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new GoogleGeocodingService(apiOptions));
        }
    }
}
