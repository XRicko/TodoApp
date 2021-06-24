using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Queries.TodoItems;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Controllers;

using Xunit;

namespace WebApi.Tests.Controllers
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

            var claim = new Claim(ClaimTypes.NameIdentifier, userId.ToString());

            contextMock.Setup(x => x.User.FindFirst(ClaimTypes.NameIdentifier))
                       .Returns(claim);
        }

        [Fact]
        public async Task Get_ReturnsNewListOfTodoItemResponsesByUserAndSetsCache()
        {
            var expected = GetSampleTodoItemResponsesByUser();

            MediatorMock.Setup(x => x.Send(It.Is<GetTodoItemsByUserIdQuery>(q => q.UserId == userId),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected)
                        .Verifiable();

            // Act
            var actual = await todoItemsController.Get();
            var cached = JsonSerializer.Deserialize<List<TodoItemResponse>>(cache.GetString(recordKey));

            // Assert
            actual.Should().Equal(expected);
            cached.Should().Equal(expected);

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
            actual.Should().Equal(expected);
            MediatorMock.Verify(x => x.Send(It.Is<GetTodoItemsByUserIdQuery>(q => q.UserId == userId),
                                            It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Get_ReturnsChecklistResponseGivenExistingId()
        {
            // Arrange
            int id = 11;
            TodoItemResponse expected = new(id, "smth", DateTime.Now, 12, "Chores", 2, "Ongoing");

            MediatorMock.Setup(x => x.Send(It.Is<GetByIdQuery<TodoItem, TodoItemResponse>>(q => q.Id == id),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected)
                        .Verifiable();

            // Act
            var actual = await todoItemsController.Get(id);

            // Assert
            actual.Value.Should().Be(expected);
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Get_ReturnsChecklistResponseGivenInvalidId()
        {
            // Arrange
            int id = 11;

            MediatorMock.Setup(x => x.Send(It.Is<GetByIdQuery<TodoItem, TodoItemResponse>>(q => q.Id == id),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => null)
                        .Verifiable();

            // Act
            var actual = await todoItemsController.Get(id);

            // Assert
            actual.Value.Should().BeNull();
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Add_SendsRequest()
        {
            // Arrange
            var createRequest = new TodoItemCreateRequest("Essential", 2, 1, DateTime.Now.AddDays(2));
            var responses = GetSampleTodoItemResponsesByUser();

            cache.SetString(recordKey, JsonSerializer.Serialize(responses));

            // Act
            var result = await todoItemsController.Add(createRequest);

            // Assert
            cache.Get(recordKey).Should().BeNull();
            result.Should().BeAssignableTo<NoContentResult>();

            MediatorMock.Verify(x => x.Send(It.Is<AddCommand<TodoItemCreateRequest>>(q => q.Request == createRequest),
                                            It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_SendsRequest()
        {
            // Arrange
            int id = 3;
            var responses = GetSampleTodoItemResponsesByUser();

            cache.SetString(recordKey, JsonSerializer.Serialize(responses));

            // Act
            var result = await todoItemsController.Delete(id);

            // Assert
            cache.Get(recordKey).Should().BeNull();
            result.Should().BeAssignableTo<NoContentResult>();

            MediatorMock.Verify(x => x.Send(It.Is<RemoveCommand<TodoItem>>(q => q.Id == id),
                                            It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_SendsRequest()
        {
            // Arrange
            var updateRequest = new TodoItemUpdateRequest(2, "Cleaninig", 2, 1, DateTime.Now);
            var responses = GetSampleTodoItemResponsesByUser();

            cache.SetString(recordKey, JsonSerializer.Serialize(responses));

            // Act
            var result = await todoItemsController.Update(updateRequest);

            // Assert
            cache.Get(recordKey).Should().BeNull();
            result.Should().BeAssignableTo<NoContentResult>();

            MediatorMock.Verify(x => x.Send(It.Is<UpdateCommand<TodoItemUpdateRequest>>(q => q.Request == updateRequest),
                                            It.IsAny<CancellationToken>()), Times.Once);
        }

        private static List<TodoItemResponse> GetSampleTodoItemResponsesByUser()
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
