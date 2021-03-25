using System;
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
    public class TodoItemsControllerTests : ControllerBaseForTests
    {
        private readonly TodoItemsController todoItemsController;

        public TodoItemsControllerTests() : base()
        {
            todoItemsController = new TodoItemsController(MediatorMock.Object);
        }

        [Fact]
        public async Task Add_SendsRequest()
        {
            // Arrange
            var createRequest = new TodoItemCreateRequest("Essential", 2, 1, DateTime.Now.AddDays(2));

            MediatorMock.Setup(x => x.Send(It.Is<AddCommand<TodoItemCreateRequest>>(q => q.Request == createRequest), It.IsAny<CancellationToken>()));

            // Act
            await todoItemsController.Add(createRequest);

            // Assert
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Delete_SendsRequest()
        {
            // Arrange
            int id = 3;

            MediatorMock.Setup(x => x.Send(It.Is<RemoveCommand<TodoItem>>(q => q.Id == id), It.IsAny<CancellationToken>()));

            // Act
            await todoItemsController.Delete(id);

            // Assert
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Update_SendsRequest()
        {
            // Arrange
            var updateRequest = new TodoItemUpdateRequest(2, "Cleaninig", 2, 1, DateTime.Now);

            MediatorMock.Setup(x => x.Send(It.Is<UpdateCommand<ChecklistUpdateRequest>>(q => q.Request == updateRequest), It.IsAny<CancellationToken>()));

            // Act
            await todoItemsController.Update(updateRequest);

            // Assert
            MediatorMock.Verify();
        }
    }
}
