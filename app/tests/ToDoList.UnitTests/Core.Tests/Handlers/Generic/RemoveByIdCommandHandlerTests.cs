using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Handlers.TodoItems;

using Xunit;

namespace Core.Tests.Handlers.Generic
{
    public class RemoveByIdCommandHandlerTests : HandlerBaseForTests
    {
        private readonly RemoveTodoItemByIdCommandHandler removeCommandHandler;

        private readonly int id;
        private readonly string name;

        private readonly IQueryable<TodoItem> todoItems;

        public RemoveByIdCommandHandlerTests() : base()
        {
            removeCommandHandler = new RemoveTodoItemByIdCommandHandler(UnitOfWorkMock.Object, Mapper);

            id = 420;
            name = "Clean my room";

            todoItems = new List<TodoItem>
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
            }.AsQueryable();

            RepoMock.Setup(x => x.GetAll<TodoItem>())
                    .Returns(todoItems)
                    .Verifiable();
        }

        [Fact]
        public async Task Handle_DeletesItemGivenExisting()
        {
            // Act
            await removeCommandHandler.Handle(new RemoveByIdCommand<TodoItem>(id), new CancellationToken());

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
            await removeCommandHandler.Handle(new RemoveByIdCommand<TodoItem>(invalidId), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Remove(It.IsAny<TodoItem>()), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }
    }
}
