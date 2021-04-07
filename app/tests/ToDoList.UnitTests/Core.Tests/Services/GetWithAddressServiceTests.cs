using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Mediator.Response;
using ToDoList.Core.Services;
using ToDoList.SharedKernel;

using Xunit;

namespace Core.Tests.Services
{
    public class GetWithAddressServiceTests
    {
        private readonly Mock<IGeocodingService> geocodingMock;
        private readonly CreateWithAddressService addressService;

        public GetWithAddressServiceTests()
        {
            geocodingMock = new Mock<IGeocodingService>();
            addressService = new CreateWithAddressService(geocodingMock.Object);
        }

        [Fact]
        public async Task GetItemWithAddressAsync_ReturnsWithAddressGivenWithGeoPoint()
        {
            // Arrange
            double latitude = 49.06802;
            double longitude = 33.42041;

            var todoRespones = new List<TodoItemResponse>
            {
                new(0, "Party", DateTime.Now, 2, "Birthday", 1, "Planned", null, new GeoCoordinate(longitude, latitude)),
                new(0, "Party", DateTime.Now, 2, "Birthday", 1, "Planned")
            };
            var expected = GetWithAddress(todoRespones);

            geocodingMock.Setup(x => x.GetAddressAsync(latitude, longitude))
                .ReturnsAsync("Khalamenyuka St, 4, Kremenchuk, Poltavs'ka oblast, Ukraine, 39600");

            // Act
            var actual = await addressService.GetItemsWithAddressAsync(todoRespones);

            // Assert
            Assert.Equal(expected, actual);
            geocodingMock.Verify(x => x.GetAddressAsync(latitude, longitude), Times.Once);
        }

        private static IEnumerable<TodoItemResponse> GetWithAddress(IEnumerable<TodoItemResponse> responses)
        {
            List<TodoItemResponse> expected = new();

            foreach (var response in responses)
            {
                if (response.GeoPoint is not null && response.GeoPoint.Latitude == 49.06802 && response.GeoPoint.Longitude == 33.42041)
                {
                    expected.Add(response with { Address = "Khalamenyuka St, 4, Kremenchuk, Poltavs'ka oblast, Ukraine, 39600" });
                }
                else
                    expected.Add(response);
            }

            return expected;
        }
    }
}
