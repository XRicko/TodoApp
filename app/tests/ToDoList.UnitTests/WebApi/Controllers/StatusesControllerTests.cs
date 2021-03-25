using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Controllers;

using Xunit;

namespace ToDoList.UnitTests.WebApi.Controllers
{
    public class StatusesControllerTests : ControllerBaseForTests
    {
        private readonly StatusesController statusesController;

        public StatusesControllerTests() : base()
        {
            statusesController = new StatusesController(MediatorMock.Object);
        }

        [Fact]
        public async Task Get_ReturnsListOfCategoryResponses()
        {
            // Arrange
            var expected = new List<StatusResponse>
            {
                new(1, "Planned", false),
                new(1, "Ongoing", false),
                new(1, "Done", true)
            };

            MediatorMock.Setup(x => x.Send(It.IsAny<GetAllQuery<Status, StatusResponse>>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected);

            // Act
            var actual = await statusesController.Get();

            // Assert
            Assert.Equal(expected, actual);
            MediatorMock.Verify();
        }

        [Fact]
        public async Task GetByName_ReturnsStatusResponeGivenExistingName()
        {
            // Arrange
            string name = "Planned";
            var expected = new StatusResponse(2, name, false);

            MediatorMock.Setup(x => x.Send(It.Is<GetByNameQuery<Status, StatusResponse>>(q => q.Name == name), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected);

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
                      .ReturnsAsync(() => null);

            // Act
            var actual = await statusesController.GetByName(invalidName);

            // Assert
            Assert.Null(actual);
            MediatorMock.Verify();
        }
    }
}
