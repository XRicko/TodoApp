using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Controllers;

using Xunit;

namespace ToDoList.UnitTests.WebApi.Controllers
{
    public class StatusesControllerTests : ApiControllerBaseForTests
    {
        private readonly IDistributedCache cache;
        private readonly StatusesController statusesController;

        private readonly string recordKey;

        public StatusesControllerTests() : base()
        {
            var opts = Options.Create(new MemoryDistributedCacheOptions());
            cache = new MemoryDistributedCache(opts);
            
            statusesController = new StatusesController(MediatorMock.Object, cache);

            recordKey = "Statuses";
        }

        [Fact]
        public async Task Get_ReturnsNewListOfStatusResponsesAndSetsCache()
        {
            // Arrange
            var expected = GetSampleStatusResponses();

            MediatorMock.Setup(x => x.Send(It.IsAny<GetAllQuery<Status, StatusResponse>>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected)
                        .Verifiable();

            // Act
            var actual = await statusesController.Get();
            var cached = JsonSerializer.Deserialize<List<StatusResponse>>(cache.GetString(recordKey));

            // Assert
            Assert.Equal(expected, actual);
            Assert.Equal(cached, expected);

            MediatorMock.Verify();
        }

        [Fact]
        public async Task Get_ReturnsListOfStatusResponsesFromCache()
        {
            // Arrange
            var expected = GetSampleStatusResponses();
            cache.SetString(recordKey, JsonSerializer.Serialize(expected));

            // Act
            var actual = await statusesController.Get();

            // Assert
            Assert.Equal(expected, actual);
            MediatorMock.Verify(x => x.Send(It.IsAny<GetAllQuery<Status, StatusResponse>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetByName_ReturnsStatusResponeGivenExistingName()
        {
            // Arrange
            string name = "Planned";
            var expected = new StatusResponse(2, name, false);

            MediatorMock.Setup(x => x.Send(It.Is<GetByNameQuery<Status, StatusResponse>>(q => q.Name == name), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected)
                        .Verifiable();

            // Act
            var actual = await statusesController.GetByName(name);

            // Assert
            Assert.Equal(expected, actual);
            MediatorMock.Verify();
        }

        [Fact]
        public async Task GetByName_ReturnsNullGivenInvalidName()
        {
            // Arrange
            string invalidName = "invalid_name";

            MediatorMock.Setup(x => x.Send(It.Is<GetByNameQuery<Status, StatusResponse>>(q => q.Name == invalidName), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => null)
                        .Verifiable();

            // Act
            var actual = await statusesController.GetByName(invalidName);

            // Assert
            Assert.Null(actual);
            MediatorMock.Verify();
        }

        private static List<StatusResponse> GetSampleStatusResponses()
        {
            return new List<StatusResponse>
            {
                new(1, "Planned", false),
                new(1, "Ongoing", false),
                new(1, "Done", true)
            };
        }
    }
}
