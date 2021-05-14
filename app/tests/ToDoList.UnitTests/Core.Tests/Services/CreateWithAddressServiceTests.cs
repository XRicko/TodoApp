using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using Moq;

using ToDoList.Core.Mediator.Response;
using ToDoList.Core.Services;
using ToDoList.SharedKernel;

using Xunit;

namespace Core.Tests.Services
{
    public class CreateWithAddressServiceTests
    {
        private readonly Mock<IGeocodingService> geocodingMock;

        private readonly IDistributedCache cache;
        private readonly CreateWithAddressService addressService;

        private readonly string address;

        private readonly double latitude;
        private readonly double longitude;

        public CreateWithAddressServiceTests()
        {
            var opts = Options.Create(new MemoryDistributedCacheOptions());

            geocodingMock = new Mock<IGeocodingService>();

            cache = new MemoryDistributedCache(opts);
            addressService = new CreateWithAddressService(geocodingMock.Object, cache);

            address = "Khalamenyuka St, 4, Kremenchuk, Poltavs'ka oblast, Ukraine, 39600";

            latitude = 49.06802;
            longitude = 33.42041;
        }

        [Fact]
        public async Task GetItemsWithAddressAsync_ReturnsItemWithAddressGivenGeoPoint()
        {
            // Arrange
            var todoRespones = new List<TodoItemResponse>
            {
                new(0, "Party", DateTime.Now, 2, "Birthday", 1, "Planned", null, new GeoCoordinate(longitude, latitude)),
                new(0, "Party", DateTime.Now, 2, "Birthday", 1, "Planned")
            };

            var expected = GetWithAddress(todoRespones);

            geocodingMock.Setup(x => x.GetAddressAsync(latitude, longitude))
                         .ReturnsAsync(address)
                         .Verifiable();

            // Act
            var actual = await addressService.GetItemsWithAddressAsync(todoRespones);

            // Assert
            Assert.Equal(expected, actual);
            geocodingMock.Verify();
        }

        [Fact]
        public async Task GetItemWithAddressAsync_ReturnsFromCacheIfExistsGivenGeoPoint()
        {
            // Arrange
            TodoItemResponse todoItemResponse = new(0, "Party", DateTime.Now, 2, "Birthday", 1, "Planned", null, new GeoCoordinate(longitude, latitude));

            var expected = todoItemResponse with { Address = address };

            string recordKey = $"Address_{todoItemResponse.GeoPoint.Latitude}_{todoItemResponse.GeoPoint.Longitude}";
            cache.SetString(recordKey, JsonSerializer.Serialize(address));

            // Act
            var actual = await addressService.GetItemWithAddressAsync(todoItemResponse);

            // Assert
            Assert.Equal(expected, actual);
            geocodingMock.Verify(x => x.GetAddressAsync(latitude, longitude), Times.Never);
        }

        [Fact]
        public async Task GetItemWithAddressAsync_ReturnsFromGeocodingApiIfNoCachedGivenGeoPoint()
        {
            // Arrange
            TodoItemResponse todoItemResponse = new(0, "Party", DateTime.Now, 2, "Birthday", 1, "Planned", null, new GeoCoordinate(longitude, latitude));

            var expected = todoItemResponse with { Address = address };

            geocodingMock.Setup(x => x.GetAddressAsync(latitude, longitude))
                         .ReturnsAsync(address)
                         .Verifiable();

            // Act
            var actual = await addressService.GetItemWithAddressAsync(todoItemResponse);

            // Assert
            Assert.Equal(expected, actual);
            geocodingMock.Verify();
        }

        private IEnumerable<TodoItemResponse> GetWithAddress(IEnumerable<TodoItemResponse> responses)
        {
            List<TodoItemResponse> expected = new();

            foreach (var response in responses)
            {
                if (response.GeoPoint is not null && response.GeoPoint.Latitude == 49.06802 && response.GeoPoint.Longitude == 33.42041)
                {
                    expected.Add(response with { Address = address });
                }
                else
                    expected.Add(response);
            }

            return expected;
        }
    }
}
