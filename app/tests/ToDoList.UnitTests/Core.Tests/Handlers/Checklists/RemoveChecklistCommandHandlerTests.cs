using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MockQueryable.Moq;

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
        private readonly string defaultName;

        private readonly List<Checklist> checklists;
        private readonly Mock<IQueryable<Checklist>> checklistsMock;

        public RemoveChecklistCommandHandlerTests() : base()
        {
            removeChecklistHandler = new RemoveChecklistCommandHandler(UnitOfWorkMock.Object, Mapper);

            id = 9;
            defaultName = "Untitled";

            checklists = GetSampleChecklists();
            checklistsMock = checklists.AsQueryable().BuildMock();
        }

        [Fact]
        public async Task Handle_DeletesChecklistWithTodosGivenExistingWithProperId()
        {
            // Arrange
            var checklistToDelete = checklists.SingleOrDefault(x => x.Id == id);

            RepoMock.Setup(x => x.GetAll<Checklist>())
                    .Returns(checklistsMock.Object)
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
                    .Returns(checklistsMock.Object)
                    .Verifiable();

            // Act
            await removeChecklistHandler.Handle(new RemoveCommand<Checklist>(invalidId), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Remove(It.IsAny<Checklist>()), Times.Never);
            RepoMock.Verify(x => x.Remove(It.IsAny<TodoItem>()), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_DoesntDeleteChecklistGivenDefaultName()
        {
            // Arrange
            int defaultId = 1;
            var defaultChecklist = checklists.SingleOrDefault(x => x.Name == defaultName);

            RepoMock.Setup(x => x.GetAll<Checklist>())
                    .Returns(checklistsMock.Object);

            // Act
            await removeChecklistHandler.Handle(new RemoveCommand<Checklist>(defaultId), new CancellationToken());

            // Assert
            RepoMock.Verify(x => x.Remove(It.IsAny<Checklist>()), Times.Never);
            RepoMock.Verify(x => x.Remove(It.Is<TodoItem>(i => i.ChecklistId == defaultId)), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
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
                },
                new Checklist
                {
                    Id = 1,
                    Name = defaultName,

                    TodoItems = new List<TodoItem>
                    {
                        new TodoItem {Name = "Something", ChecklistId = 1}
                    }
                }
            };
        }
    }
}
