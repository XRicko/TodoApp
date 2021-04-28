using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MockQueryable.Moq;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Handlers.TodoItems;

using Xunit;

namespace Core.Tests.Handlers.Generic
{
    public class RemoveCommandHandlerTests : HandlerBaseForTests
    {
        private readonly RemoveTodoItemCommandHandler removeCommandHandler;

        private readonly int id;
        private readonly string name;

        private readonly Mock<IQueryable<TodoItem>> todoItemsMock;

        public RemoveCommandHandlerTests() : base()
        {
            removeCommandHandler = new RemoveTodoItemCommandHandler(UnitOfWorkMock.Object, Mapper);

            id = 420;
            name = "Clean my room";

            var todoItems = GetSampleTodoItems();
            todoItemsMock = todoItems.AsQueryable().BuildMock();

            RepoMock.Setup(x => x.GetAll<TodoItem>())
                    .Returns(todoItemsMock.Object)
                    .Verifiable();
        }

        [Fact]
        public async Task Handle_DeletesItemGivenExisting()
        {
            // Act
            await removeCommandHandler.Handle(new RemoveCommand<TodoItem>(id), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Remove(It.Is<TodoItem>(x => x.Id == id)), Times.Once);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
        }


        [Fact]
        public async Task Handle_DoesntDeleteItemGivenInvalidId()
        {
            // Arrange
            int invalidId = 13;

            // Act
            await removeCommandHandler.Handle(new RemoveCommand<TodoItem>(invalidId), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Remove(It.IsAny<TodoItem>()), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }

        private List<TodoItem> GetSampleTodoItems()
        {
            return new List<TodoItem>
            {
                new TodoItem
                {
                    Id = id,
                    Name = name
                },
                new TodoItem
                {
                    Id = 12,
                    Name = "Something random"
                }
            };
        }
    }
}
