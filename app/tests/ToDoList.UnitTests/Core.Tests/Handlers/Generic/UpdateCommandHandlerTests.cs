using System;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Handlers.TodoItems;
using ToDoList.Core.Mediator.Requests.Update;

using Xunit;

namespace Core.Tests.Handlers.Generic
{
    public class UpdateCommandHandlerTests : HandlerBaseForTests
    {
        private readonly UpdateTodoItemCommandHandler updateCommandHandler;

        public UpdateCommandHandlerTests() : base()
        {
            updateCommandHandler = new UpdateTodoItemCommandHandler(UnitOfWorkMock.Object, Mapper);
        }

        [Fact]
        public async Task Handle_UpdatesItem()
        {
            // Arrange
            var request = new TodoItemUpdateRequest(2, "Do something", 6, 2, DateTime.Now);

            // Act
            await updateCommandHandler.Handle(new UpdateCommand<TodoItemUpdateRequest>(request), new CancellationToken());

            // Assert
            RepoMock.Verify(x => x.Update(It.Is<TodoItem>(l => l.Id == 2)), Times.Once);
            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);

        }
    }
}
