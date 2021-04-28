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

        private readonly double latitude;
        private readonly double longitude;

        public GetWithAddressServiceTests()
        {
            geocodingMock = new Mock<IGeocodingService>();
            addressService = new CreateWithAddressService(geocodingMock.Object);

            latitude = 49.06802;
            longitude = 33.42041;

            geocodingMock.Setup(x => x.GetAddressAsync(latitude, longitude))
                         .ReturnsAsync("Khalamenyuka St, 4, Kremenchuk, Poltavs'ka oblast, Ukraine, 39600");
        }

        //[Fact]
        //public async Task GetItemsWithAddressAsync_ReturnsItemWithAddressGivenGeoPoint()
        //{
        //    // Arrange
        //    var todoRespones = new List<TodoItemResponse>
        //    {
        //        new(0, "Party", DateTime.Now, 2, "Birthday", 1, "Planned", null, new GeoCoordinate(longitude, latitude)),
        //        new(0, "Party", DateTime.Now, 2, "Birthday", 1, "Planned")
        //    };
        //    var expected = GetWithAddress(todoRespones);

        //    // Act
        //    var actual = await addressService.GetItemsWithAddressAsync(todoRespones);

        //    // Assert
        //    Assert.Equal(expected, actual);
        //    geocodingMock.Verify(x => x.GetAddressAsync(latitude, longitude), Times.Once);
        //}

        //[Fact]
        //public async Task GetItemWithAddressAsync_ReturnsItemWithAddressGivenGeoPoint()
        //{
        //    // Arrange
        //    TodoItemResponse todoItemResponse = new(0, "Party", DateTime.Now, 2, "Birthday", 1, "Planned", null, new GeoCoordinate(longitude, latitude));

        //    var expected = todoItemResponse with { Address = "Khalamenyuka St, 4, Kremenchuk, Poltavs'ka oblast, Ukraine, 39600" };

        //    // Act
        //    var actual = await addressService.GetItemWithAddressAsync(todoItemResponse);

        //    // Assert
        //    Assert.Equal(expected, actual);
        //    geocodingMock.Verify(x => x.GetAddressAsync(latitude, longitude), Times.Once);
        //}

        //private static IEnumerable<TodoItemResponse> GetWithAddress(IEnumerable<TodoItemResponse> responses)
        //{
        //    List<TodoItemResponse> expected = new();

        //    foreach (var response in responses)
        //    {
        //        if (response.GeoPoint is not null && response.GeoPoint.Latitude == 49.06802 && response.GeoPoint.Longitude == 33.42041)
        //        {
        //            expected.Add(response with { Address = "Khalamenyuka St, 4, Kremenchuk, Poltavs'ka oblast, Ukraine, 39600" });
        //        }
        //        else
        //            expected.Add(response);
        //    }

        //    return expected;
        //}
    }
}
