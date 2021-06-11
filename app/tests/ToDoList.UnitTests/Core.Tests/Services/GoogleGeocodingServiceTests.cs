using System;

using FluentAssertions;

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

            // Act
            Action action = () =>
            {
                var googleGeocodingService = new GoogleGeocodingService(apiOptions);
            };

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }
    }
}
