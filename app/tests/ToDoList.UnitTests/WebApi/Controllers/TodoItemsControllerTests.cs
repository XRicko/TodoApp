using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Queries.TodoItems;
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
        public async Task Get_ReturnsListOfActiveTodoItemResponsesByUser()
        {
            await TestGet(4, false);
        }

        [Fact]
        public async Task Get_ReturnsListOfDoneTodoItemResponsesByUser()
        {
            await TestGet(2, true);
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

        private async Task TestGet(int userId, bool isDone)
        {
            var contextMock = new Mock<HttpContext>();
            todoItemsController.ControllerContext.HttpContext = contextMock.Object;

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };

            contextMock.SetupGet(x => x.User.Claims)
                       .Returns(claims);

            // Act
            var actual = await todoItemsController.GetActiveOrDone(isDone);

            // Assert
            MediatorMock.Verify(x => x.Send(It.Is<GetActiveOrDoneTodoItemsByUserQuery>(q => q.UserId == userId
                                                                                           && q.IsDone == isDone),
                                            It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
