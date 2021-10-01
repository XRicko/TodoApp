using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.TodoItems;
using ToDoList.Core.Mediator.Queries.TodoItems;
using ToDoList.Core.Mediator.Response;
using ToDoList.Core.Services;

using Xunit;

namespace Core.Tests.Handlers.TodoItems
{
    public class GetTodoItemsByChecklistIdQueryHandlerTests : HandlerBaseForTests
    {
        private readonly GetTodoItemsByChecklistIdQueryHandler handler;
        private readonly Mock<IAddressService> addressServiceMock;

        private readonly int checklistId;

        public GetTodoItemsByChecklistIdQueryHandlerTests()
        {
            addressServiceMock = new Mock<IAddressService>();
            Mock<IFileSystem> fileSystemMock = new();

            handler = new GetTodoItemsByChecklistIdQueryHandler(UnitOfWorkMock.Object, Mapper, addressServiceMock.Object, fileSystemMock.Object);

            checklistId = 22;
        }

        [Fact]
        public async Task Handle_ReturnsTodoItems()
        {
            // Arrange
            var todoItems = GetSampleTodoItems().AsQueryable();

            var expected = new List<TodoItemResponse>
            {
                new(21, "Complete some course", DateTime.Now, 22, "Chores", 2, "Planned")
            };

            RepoMock.Setup(x => x.GetAll<TodoItem>())
                    .Returns(todoItems)
                    .Verifiable();

            addressServiceMock.Setup(x => x.GetItemsWithAddressAsync(It.IsAny<IEnumerable<TodoItemResponse>>()))
                              .ReturnsAsync(expected)
                              .Verifiable();

            // Act
            var actual = await handler.Handle(new GetTodoItemsByChecklistIdQuery(checklistId), new CancellationToken());

            // Assert
            actual.Should().Equal(expected);

            RepoMock.Verify();
            addressServiceMock.Verify();
        }

        private IEnumerable<TodoItem> GetSampleTodoItems()
        {
            var checklist1 = new Checklist { Id = 12, Name = "Chores", ProjectId = 4 };
            var checklist2 = new Checklist { Id = checklistId, Name = "Chores", ProjectId = 12 };

            var todoItems = new List<TodoItem>
            {
                new TodoItem
                {
                    Id = 21,
                    Name = "Complete some course",
                    Checklist = checklist2,
                    Status = new Status { Id = 2, Name = "Planned" },
                    Category = new Category(),
                    Image = new Image()
                },
                new TodoItem
                {
                    Id = 12,
                    Name = "Finish the book",
                    Checklist = checklist1,
                    Status = new Status(),
                    Category = new Category(),
                    Image = new Image()
                }
            };

            return todoItems;
        }
    }
}
