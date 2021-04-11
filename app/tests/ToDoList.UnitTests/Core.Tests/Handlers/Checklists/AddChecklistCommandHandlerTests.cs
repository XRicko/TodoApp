using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
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
            var checklists = GetSampleChecklists();

            RepoMock.Setup(x => x.GetAllAsync<Checklist>())
                    .ReturnsAsync(checklists);

            // Act
            await addChecklistHandler.Handle(new AddCommand<ChecklistCreateRequest>(newChecklist), new CancellationToken());

            // Assert
            RepoMock.Verify(x => x.GetAllAsync<Checklist>(), Times.Once);
            RepoMock.Verify(x => x.AddAsync(It.Is<Checklist>(l => l.Name == newChecklist.Name
                                                                  && l.UserId == newChecklist.UserId)), Times.Once);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DoesntAddChecklistGivenExisting()
        {
            // Arrange
            int userId = 2;

            var existingChecklist = new ChecklistCreateRequest(name, userId);
            var checklists = GetSampleChecklists();

            RepoMock.Setup(x => x.GetAllAsync<Checklist>())
                    .ReturnsAsync(checklists);

            // Act
            await addChecklistHandler.Handle(new AddCommand<ChecklistCreateRequest>(existingChecklist), new CancellationToken());

            // Assert
            RepoMock.Verify(x => x.GetAllAsync<Checklist>(), Times.Once);
            RepoMock.Verify(x => x.AddAsync(It.Is<Checklist>(l => l.Name == existingChecklist.Name
                                                                  && l.UserId == existingChecklist.UserId)), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }

        private IEnumerable<Checklist> GetSampleChecklists()
        {
            return new List<Checklist>
            {
                new Checklist { Id = 1, Name = "Chores", UserId = 2 },
                new Checklist { Id = 3, Name = "Work", UserId = 2 },
                new Checklist { Id = 4, Name = "Chores", UserId = 1 },
                new Checklist { Id = 5, Name = name, UserId = 2 },
            };
        }
    }
}
