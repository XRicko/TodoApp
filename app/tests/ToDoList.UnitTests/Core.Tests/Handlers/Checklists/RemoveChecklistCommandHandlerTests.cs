using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Handlers.Checklists;

using Xunit;

namespace Core.Tests.Handlers.Checklists
{
    public class RemoveChecklistCommandHandlerTests : HandlerBaseForTests
    {
        private readonly RemoveChecklistCommandHandler removeChecklistHandler;

        private readonly int id;

        public RemoveChecklistCommandHandlerTests() : base()
        {
            removeChecklistHandler = new RemoveChecklistCommandHandler(UnitOfWorkMock.Object, Mapper);

            id = 9;
        }

        [Fact]
        public async Task DeletesChecklistWithTodosGivenExistingWithProperName()
        {
            // Arrange
            var entity = GetSampleChecklist("Birthday");

            RepoMock.Setup(x => x.GetAsync<Checklist>(id))
                   .ReturnsAsync(entity);

            // Act
            await removeChecklistHandler.Handle(new RemoveCommand<Checklist>(id), new CancellationToken());

            // Arrange
            RepoMock.Verify(x => x.GetAsync<Checklist>(id), Times.Once);
            RepoMock.Verify(x => x.Remove(entity), Times.Once);
            RepoMock.Verify(x => x.Remove(It.Is<TodoItem>(i => i.ChecklistId == id)), Times.Exactly(entity.TodoItems.Count));

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DoesntDeleteChecklistGivenInvalidId()
        {
            RepoMock.Setup(x => x.GetAsync<TodoItem>(id))
                             .ReturnsAsync(() => null);

            // Act
            await removeChecklistHandler.Handle(new RemoveCommand<Checklist>(id), new CancellationToken());

            // Assert
            RepoMock.Verify(x => x.GetAsync<Checklist>(id), Times.Once);
            RepoMock.Verify(x => x.Remove(It.IsAny<Checklist>()), Times.Never);
            RepoMock.Verify(x => x.Remove(It.IsAny<TodoItem>()), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task DoesntDeleteChecklistGivenInvalidName()
        {
            // Arrange
            var entity = GetSampleChecklist("Untitled");

            RepoMock.Setup(x => x.GetAsync<Checklist>(id))
                  .ReturnsAsync(entity);

            // Act
            await removeChecklistHandler.Handle(new RemoveCommand<Checklist>(id), new CancellationToken());

            // Assert
            RepoMock.Verify(x => x.GetAsync<Checklist>(id), Times.Once);
            RepoMock.Verify(x => x.Remove(It.IsAny<Checklist>()), Times.Never);
            RepoMock.Verify(x => x.Remove(It.Is<TodoItem>(i => i.ChecklistId == id)), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }

        private Checklist GetSampleChecklist(string name)
        {
            return new Checklist
            {
                Id = id,
                Name = name,

                TodoItems = new List<TodoItem>
                {
                    new TodoItem { Name = "Invite friends", ChecklistId = id },
                    new TodoItem { Name = "Prepare a party", ChecklistId = id },
                    new TodoItem { Name = "Buy everyting needed", ChecklistId = id }
                }
            };
        }
    }
}
