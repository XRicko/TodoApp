using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Handlers.Checklists;
using ToDoList.Core.Mediator.Handlers.TodoItems;
using ToDoList.Core.Mediator.Requests.Update;

using Xunit;

namespace Core.Handlers.Generic
{
    public class UpdateCommandHandlerTest : HandlerBaseForTests
    {
        private readonly UpdateChecklistCommandHandler updateCommandHandler;

        public UpdateCommandHandlerTest() : base()
        {
            updateCommandHandler = new UpdateChecklistCommandHandler(UnitOfWorkMock.Object, Mapper);
        }

        [Fact]
        public async Task UpdatesItem()
        {
            // Arrange
            var request = new ChecklistUpdateRequest(2, "Birthday", 1);

            // Act
            await updateCommandHandler.Handle(new UpdateCommand<ChecklistUpdateRequest>(request), new CancellationToken());

            // Assert
            RepoMock.Verify(x => x.Update(It.Is<Checklist>(l => l.Id == 2)), Times.Once);
            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
        }
    }
}
