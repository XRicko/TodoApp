using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MockQueryable.Moq;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Handlers.Checklists;
using ToDoList.Core.Mediator.Requests.Create;

using Xunit;

namespace Core.Tests.Handlers.Checklists
{
    public class AddChecklistCommandHandlerTests : HandlerBaseForTests
    {
        private readonly AddChecklistCommandHandler addChecklistHandler;

        private readonly string name;

        public AddChecklistCommandHandlerTests() : base()
        {
            addChecklistHandler = new AddChecklistCommandHandler(UnitOfWorkMock.Object, Mapper);

            name = "Birthday";
        }

        [Fact]
        public async Task Handle_AddsChecklistGivenNew()
        {
            // Arrange
            int userId = 1;

            var newChecklist = new ChecklistCreateRequest(name, userId);
            var checklists = new List<Checklist> { new Checklist { Id = 12, Name = "Chores" } };

            var checklistsMock = checklists.AsQueryable().BuildMock();

            RepoMock.Setup(x => x.GetAll<Checklist>())
                    .Returns(checklistsMock.Object)
                    .Verifiable();

            // Act
            await addChecklistHandler.Handle(new AddCommand<ChecklistCreateRequest>(newChecklist), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Add(It.Is<Checklist>(e => e.Name == name && e.UserId == userId)), Times.Once);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DoesntAddChecklistGivenExisting()
        {
            // Arrange
            int userId = 2;

            var existingChecklist = new Checklist { Id = 3, Name = name, UserId = userId };
            var request = new ChecklistCreateRequest(name, userId);

            var checklists = new List<Checklist> { existingChecklist };
            var checklistsMock = checklists.AsQueryable().BuildMock();

            RepoMock.Setup(x => x.GetAll<Checklist>())
                    .Returns(checklistsMock.Object)
                    .Verifiable();

            // Act
            await addChecklistHandler.Handle(new AddCommand<ChecklistCreateRequest>(request), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Add(It.Is<Checklist>(e => e.Name == name && e.UserId == userId)), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }
    }
}
