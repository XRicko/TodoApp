using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Queries.TodoItems;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Controllers;

using Xunit;

namespace ToDoList.UnitTests.WebApi.Controllers
{
    public class TodoItemsControllerTests : ApiControllerBaseForTests
    {
        private readonly IDistributedCache cache;
        private readonly TodoItemsController todoItemsController;

        private readonly int userId;
        private readonly string recordKey;

        public TodoItemsControllerTests() : base()
        {
            var opts = Options.Create(new MemoryDistributedCacheOptions());
            cache = new MemoryDistributedCache(opts);

            todoItemsController = new TodoItemsController(MediatorMock.Object, cache);

            userId = 4;
            recordKey = $"TodoItems_User_{userId}";

            var contextMock = new Mock<HttpContext>();
            todoItemsController.ControllerContext.HttpContext = contextMock.Object;

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };

            contextMock.SetupGet(x => x.User.Claims)
                       .Returns(claims);
        }

        [Fact]
        public async Task Get_ReturnsNewListOfTodoItemResponsesByUserAndSetsCache()
        {
            var expected = GetSampleTodoItemResponsesByUser();

            MediatorMock.Setup(x => x.Send(It.Is<GetTodoItemsByUserIdQuery>(q => q.UserId == userId), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected)
                        .Verifiable();

            // Act
            var actual = await todoItemsController.Get();
            var cached = JsonSerializer.Deserialize<List<TodoItemResponse>>(cache.GetString(recordKey));

            // Assert
            Assert.Equal(expected, actual);
            Assert.Equal(cached, expected);

            MediatorMock.Verify();
        }

        [Fact]
        public async Task Get_ReturnsListOfTodoItemResponsesByUserFromCache()
        {
            var expected = GetSampleTodoItemResponsesByUser();

            cache.SetString(recordKey, JsonSerializer.Serialize(expected));

            // Act
            var actual = await todoItemsController.Get();

            // Assert
            Assert.Equal(expected, actual);
            MediatorMock.Verify(x => x.Send(It.Is<GetTodoItemsByUserIdQuery>(q => q.UserId == userId), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Add_SendsRequest()
        {
            // Arrange
            var createRequest = new TodoItemCreateRequest("Essential", 2, 1, DateTime.Now.AddDays(2));
            var responses = GetSampleTodoItemResponsesByUser();

            cache.SetString(recordKey, JsonSerializer.Serialize(responses));

            MediatorMock.Setup(x => x.Send(It.Is<AddCommand<TodoItemCreateRequest>>(q => q.Request == createRequest), It.IsAny<CancellationToken>()))
                        .Verifiable();

            // Act
            await todoItemsController.Add(createRequest);

            // Assert
            Assert.Null(cache.Get(recordKey));
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Delete_SendsRequest()
        {
            // Arrange
            int id = 3;
            var responses = GetSampleTodoItemResponsesByUser();

            cache.SetString(recordKey, JsonSerializer.Serialize(responses));

            MediatorMock.Setup(x => x.Send(It.Is<RemoveCommand<TodoItem>>(q => q.Id == id), It.IsAny<CancellationToken>()))
                        .Verifiable();

            // Act
            await todoItemsController.Delete(id);

            // Assert
            Assert.Null(cache.Get(recordKey));
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Update_SendsRequest()
        {
            // Arrange
            var updateRequest = new TodoItemUpdateRequest(2, "Cleaninig", 2, 1, DateTime.Now);
            var responses = GetSampleTodoItemResponsesByUser();

            cache.SetString(recordKey, JsonSerializer.Serialize(responses));

            MediatorMock.Setup(x => x.Send(It.Is<UpdateCommand<TodoItemUpdateRequest>>(q => q.Request == updateRequest), It.IsAny<CancellationToken>()))
                        .Verifiable();

            // Act
            await todoItemsController.Update(updateRequest);

            // Assert
            Assert.Null(cache.Get(recordKey));
            MediatorMock.Verify();
        }

        private List<TodoItemResponse> GetSampleTodoItemResponsesByUser()
        {
            int checklistId = 5;

            var todoItems = new List<TodoItemResponse>
            {
                new(4123, "Do smth", DateTime.Now, checklistId, "Chores", 1, "Planned"),
                new(23, "Clean", DateTime.Now, checklistId, "Chores", 1, "Planned")
            };

            return todoItems;
        }
    }
}
