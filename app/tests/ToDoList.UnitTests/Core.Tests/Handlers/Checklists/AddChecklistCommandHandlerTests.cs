using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            int projectId = 1;

            var newChecklist = new ChecklistCreateRequest(name, projectId);

            var checklists = new List<Checklist> { new Checklist { Id = 12, Name = "Chores" } }.AsQueryable();

            RepoMock.Setup(x => x.GetAll<Checklist>())
                    .Returns(checklists)
                    .Verifiable();

            // Act
            await addChecklistHandler.Handle(new AddCommand<ChecklistCreateRequest>(newChecklist), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Add(It.Is<Checklist>(e => e.Name == name && e.ProjectId == projectId)), Times.Once);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DoesntAddChecklistGivenExisting()
        {
            // Arrange
            int projectId = 2;

            var existingChecklist = new Checklist { Id = 3, Name = name, ProjectId = projectId };
            var request = new ChecklistCreateRequest(name, projectId);

            var checklists = new List<Checklist> { existingChecklist }.AsQueryable();

            RepoMock.Setup(x => x.GetAll<Checklist>())
                    .Returns(checklists)
                    .Verifiable();

            // Act
            await addChecklistHandler.Handle(new AddCommand<ChecklistCreateRequest>(request), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Add(It.Is<Checklist>(e => e.Name == name && e.ProjectId == projectId)), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }
    }
}
