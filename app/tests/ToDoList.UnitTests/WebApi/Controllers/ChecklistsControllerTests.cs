using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.WebApi.Controllers;

using Xunit;

namespace ToDoList.UnitTests.WebApi.Controllers
{
    public class ChecklistsControllerTests : ControllerBaseForTests
    {
        private readonly ChecklistsController checklistsController;

        public ChecklistsControllerTests() : base()
        {
            checklistsController = new ChecklistsController(MediatorMock.Object);
        }

        [Fact]
        public async Task Add_SendsRequest()
        {
            // Arrange
            var createRequest = new ChecklistCreateRequest("Essential", 2);

            MediatorMock.Setup(x => x.Send(It.Is<AddCommand<ChecklistCreateRequest>>(q => q.Request == createRequest), It.IsAny<CancellationToken>()));

            // Act
            await checklistsController.Add(createRequest);

            // Assert
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Delete_SendsRequest()
        {
            // Arrange
            int id = 3;

            MediatorMock.Setup(x => x.Send(It.Is<RemoveCommand<Checklist>>(q => q.Id == id), It.IsAny<CancellationToken>()));

            // Act
            await checklistsController.Delete(id);

            // Assert
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Update_SendsRequest()
        {
            // Arrange
            var updateRequest = new ChecklistUpdateRequest(4, "Chores", 9);

            MediatorMock.Setup(x => x.Send(It.Is<UpdateCommand<ChecklistUpdateRequest>>(q => q.Request == updateRequest), It.IsAny<CancellationToken>()));

            // Act
            await checklistsController.Update(updateRequest);

            // Assert
            MediatorMock.Verify();
        }
    }
}
