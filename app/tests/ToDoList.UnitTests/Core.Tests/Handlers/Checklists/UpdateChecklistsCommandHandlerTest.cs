using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Handlers.Checklists;
using ToDoList.Core.Mediator.Requests.Update;

using Xunit;

namespace Core.Tests.Handlers.Checklists
{
    public class UpdateChecklistsCommandHandlerTest : HandlerBaseForTests
    {
        private readonly UpdateChecklistCommandHandler updateCommandHandler;

        private readonly int checklistId;
        private readonly int userId;
        private readonly string defaultChecklistName;

        private readonly ChecklistUpdateRequest request;

        public UpdateChecklistsCommandHandlerTest() : base()
        {
            updateCommandHandler = new UpdateChecklistCommandHandler(UnitOfWorkMock.Object, Mapper);

            checklistId = 2;
            userId = 10;
            defaultChecklistName = "Untitled";

            request = new ChecklistUpdateRequest(checklistId, "Birthday", userId);
        }

        [Fact]
        public async Task Handle_UpdatesChecklistAndAddsDefaultChecklistGivenUserWithoutIt()
        {
            // Arrange
            var checklists = new List<Checklist> { new Checklist { Id = 1, Name = "Chores", UserId = userId } };

            RepoMock.Setup(x => x.GetAllAsync<Checklist>())
                   .ReturnsAsync(checklists);

            // Act
            await updateCommandHandler.Handle(new UpdateCommand<ChecklistUpdateRequest>(request), new CancellationToken());

            // Assert
            RepoMock.Verify(x => x.GetAllAsync<Checklist>(), Times.Once);
            RepoMock.Verify(x => x.Update(It.Is<Checklist>(l => l.Id == checklistId)), Times.Once);
            RepoMock.Verify(x => x.AddAsync(It.Is<Checklist>(l => l.Name == defaultChecklistName && l.UserId == userId)), Times.Once);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_UpdatesChecklistAndDoesntAddDefaultChecklistGivenUserWithIt()
        {
            // Arrange
            var checklists = new List<Checklist> { new Checklist { Id = 3, Name = "Untitled", UserId = userId } };

            RepoMock.Setup(x => x.GetAllAsync<Checklist>())
                    .ReturnsAsync(checklists);

            // Act
            await updateCommandHandler.Handle(new UpdateCommand<ChecklistUpdateRequest>(request), new CancellationToken());

            // Assert
            RepoMock.Verify(x => x.GetAllAsync<Checklist>(), Times.Once);
            RepoMock.Verify(x => x.Update(It.Is<Checklist>(l => l.Id == checklistId)), Times.Once);
            RepoMock.Verify(x => x.AddAsync(It.Is<Checklist>(l => l.Name == "Untitled" && l.UserId == userId)), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
        }
    }
}
