using System.Threading;
using System.Threading.Tasks;

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

        public RemoveCommandHandlerTests() : base()
        {
            removeCommandHandler = new RemoveTodoItemCommandHandler(UnitOfWorkMock.Object, Mapper);

            id = 420;
        }

        [Fact]
        public async Task DeletesItemGivenExisting()
        {
            // Arrange
            var entity = new TodoItem { Id = id, Name = "Clean my room" };

            RepoMock.Setup(x => x.GetAsync<TodoItem>(id))
                    .ReturnsAsync(entity);

            // Act
            await removeCommandHandler.Handle(new RemoveCommand<TodoItem>(id), new CancellationToken());

            // Assert
            RepoMock.Verify(x => x.GetAsync<TodoItem>(id), Times.Once);
            RepoMock.Verify(x => x.Remove(entity), Times.Once);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
        }


        [Fact]
        public async Task DoesntDeleteItemGivenInvalidId()
        {
            // Arrange
            RepoMock.Setup(x => x.GetAsync<TodoItem>(id))
                               .ReturnsAsync(() => null);

            // Act
            await removeCommandHandler.Handle(new RemoveCommand<TodoItem>(id), new CancellationToken());

            // Assert
            RepoMock.Verify(x => x.GetAsync<TodoItem>(id), Times.Once);
            RepoMock.Verify(x => x.Remove(It.IsAny<TodoItem>()), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }
    }
}
