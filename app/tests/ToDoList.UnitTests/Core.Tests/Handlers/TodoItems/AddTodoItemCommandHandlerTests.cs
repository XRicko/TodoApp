using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Handlers.TodoItems;
using ToDoList.Core.Mediator.Requests.Create;

using Xunit;

namespace Core.Tests.Handlers.TodoItems
{
    public class AddTodoItemCommandHandlerTests : HandlerBaseForTests
    {
        private readonly AddTodoItemCommandHandler addTodoItemHandler;

        private readonly string name;

        public AddTodoItemCommandHandlerTests() : base()
        {
            addTodoItemHandler = new AddTodoItemCommandHandler(UnitOfWorkMock.Object, Mapper);

            name = "Buy everyting needed";
        }

        [Fact]
        public async Task AddsTodoItemGivenNew()
        {
            // Arrange
            var newRequest = new TodoItemCreateRequest(name, 3, 1);
            var todoItems = GetSampleTodoItems();


            RepoMock.Setup(x => x.GetAllAsync<TodoItem>())
                    .ReturnsAsync(todoItems);

            // Act
            await addTodoItemHandler.Handle(new AddCommand<TodoItemCreateRequest>(newRequest), new CancellationToken());

            // Assert
            RepoMock.Verify(x => x.GetAllAsync<TodoItem>(), Times.Once);
            RepoMock.Verify(x => x.AddAsync(It.Is<TodoItem>(i => i.Name == newRequest.Name
                                                                  && i.ChecklistId == newRequest.ChecklistId)), Times.Once);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DoesntAddTodoItemGivenExisting()
        {
            // Arrange
            var existingRequest = new TodoItemCreateRequest(name, 2, 2);
            var todoItems = GetSampleTodoItems();

            RepoMock.Setup(x => x.GetAllAsync<TodoItem>())
                    .ReturnsAsync(todoItems);

            // Act
            await addTodoItemHandler.Handle(new AddCommand<TodoItemCreateRequest>(existingRequest), new CancellationToken());

            // Assert
            RepoMock.Verify(x => x.GetAllAsync<TodoItem>(), Times.Once);
            RepoMock.Verify(x => x.AddAsync(It.Is<TodoItem>(i => i.Name == existingRequest.Name
                                                                  && i.ChecklistId == existingRequest.ChecklistId)), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }

        private IEnumerable<TodoItem> GetSampleTodoItems()
        {
            return new List<TodoItem>
            {
                new TodoItem { Name = name, ChecklistId = 2 },
                new TodoItem { Name = "Invite friends", ChecklistId = 2 },
                new TodoItem { Name = "Prepare a party", ChecklistId = 2 },
                new TodoItem { Name = name, ChecklistId = 1 }
            };
        }
    }
}
