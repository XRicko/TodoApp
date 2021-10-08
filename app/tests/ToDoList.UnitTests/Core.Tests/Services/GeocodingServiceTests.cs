using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;

using Geocoding;

using Moq;

using ToDoList.Core.Services;

using Xunit;

namespace Core.Tests.Services
{
    public class GeocodingServiceTests
    {
        private readonly GeocodingService geocodingService;
        private readonly Mock<IGeocoder> geocoderMock;

        private readonly double latitide;
        private readonly double longitude;

        public GeocodingServiceTests()
        {
            geocoderMock = new Mock<IGeocoder>();
            geocodingService = new GeocodingService(geocoderMock.Object);

            latitide = 69;
            longitude = 42.0;
        }

        [Fact]
        public async Task GetAddressAsync_ReturnsAddressGivenCoordinates()
        {
            // Arrange
            string address1 = "Unnamed road 49";
            string address2 = "Some road 27";

            Mock<Address> expectedAddressMock = new(address1, new Location(latitide, longitude), "Google");
            Mock<Address> someAddressMock = new(address2, new Location(latitide, longitude), "Google");

            var addresses = new List<Address> { expectedAddressMock.Object, someAddressMock.Object };

            expectedAddressMock.SetupGet(x => x.FormattedAddress)
                               .Returns(address1);
            someAddressMock.SetupGet(x => x.FormattedAddress)
                           .Returns(address2);

            geocoderMock.Setup(x => x.ReverseGeocodeAsync(latitide, longitude))
                        .ReturnsAsync(addresses)
                        .Verifiable();

            // Act
            string actual = await geocodingService.GetAddressAsync(latitide, longitude);

            // Assert
            actual.Should().Be(expectedAddressMock.Object.FormattedAddress);
            geocoderMock.Verify();
        }

        [Fact]
        public async Task GetAddressAsync_ReturnsMessageIfCantGetAddress()
        {
            // Arrange
            geocoderMock.Setup(x => x.ReverseGeocodeAsync(latitide, longitude))
                        .ThrowsAsync(new System.Exception())
                        .Verifiable();

            // Act
            string actual = await geocodingService.GetAddressAsync(latitide, longitude);

            // Assert
            actual.Should().Be("Unable to get address");
            geocoderMock.Verify();
        }
    }
}
