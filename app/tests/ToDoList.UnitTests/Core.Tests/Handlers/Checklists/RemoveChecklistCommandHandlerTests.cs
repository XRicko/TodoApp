using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Handlers.Checklists;

using Xunit;

namespace Core.Tests.Handlers.Checklists
{
    public class RemoveChecklistCommandHandlerTests : HandlerBaseForTests
    {
        private readonly RemoveChecklistCommandHandler removeChecklistHandler;

        private readonly int id;

        private readonly IQueryable<Checklist> checklists;

        public RemoveChecklistCommandHandlerTests() : base()
        {
            removeChecklistHandler = new RemoveChecklistCommandHandler(UnitOfWorkMock.Object, Mapper);

            id = 9;

            checklists = GetSampleChecklists().AsQueryable();
        }

        [Fact]
        public async Task Handle_DeletesChecklistWithTodosGivenExistingWithProperId()
        {
            // Arrange
            var checklistToDelete = checklists.SingleOrDefault(x => x.Id == id);

            RepoMock.Setup(x => x.GetAll<Checklist>())
                    .Returns(checklists)
                    .Verifiable();

            // Act
            await removeChecklistHandler.Handle(new RemoveCommand<Checklist>(id), new CancellationToken());

            // Arrange
            RepoMock.Verify();
            RepoMock.Verify(x => x.Remove(It.Is<Checklist>(x => x.Id == id)), Times.Once);
            RepoMock.Verify(x => x.Remove(It.Is<TodoItem>(i => i.ChecklistId == id)), Times.Exactly(checklistToDelete.TodoItems.Count));

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DoesntDeleteChecklistGivenInvalidId()
        {
            // Arrange
            int invalidId = 13456;

            RepoMock.Setup(x => x.GetAll<Checklist>())
                    .Returns(checklists)
                    .Verifiable();

            // Act
            await removeChecklistHandler.Handle(new RemoveCommand<Checklist>(invalidId), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Remove(It.IsAny<Checklist>()), Times.Never);
            RepoMock.Verify(x => x.Remove(It.IsAny<TodoItem>()), Times.Never);
        }

        private List<Checklist> GetSampleChecklists()
        {
            return new List<Checklist>
            {
                new Checklist
                {
                    Id = id,
                    Name = "Birthday",

                    TodoItems = new List<TodoItem>
                    {
                        new TodoItem { Name = "Invite friends", ChecklistId = id },
                        new TodoItem { Name = "Prepare a party", ChecklistId = id },
                        new TodoItem { Name = "Buy everyting needed", ChecklistId = id }
                    }
                },
                new Checklist
                {
                    Id = 5,
                    Name = "Chores",

                    TodoItems = new List<TodoItem>
                    {
                        new TodoItem { Name = "Clean a room", ChecklistId = 5 },
                    }
                }
            };
        }
    }
}
