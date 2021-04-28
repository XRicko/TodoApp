using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using MockQueryable.Moq;

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

        private readonly Expression<Func<Checklist, bool>> expression;

        public UpdateChecklistsCommandHandlerTest() : base()
        {
            updateCommandHandler = new UpdateChecklistCommandHandler(UnitOfWorkMock.Object, Mapper);

            checklistId = 2;
            userId = 10;
            defaultChecklistName = "Untitled";

            request = new ChecklistUpdateRequest(checklistId, "Birthday", userId);

            expression = x => x.Name == defaultChecklistName && x.UserId == userId;
        }

        [Fact]
        public async Task Handle_UpdatesChecklistAndAddsDefaultChecklistGivenUserWithoutIt()
        {
            // Arrange
            var checklists = new List<Checklist> { new Checklist { Id = 13, Name = "Chores", UserId = 134 } };
            var checklistsMock = checklists.AsQueryable().BuildMock();

            RepoMock.Setup(x => x.GetAll<Checklist>())
                    .Returns(checklistsMock.Object)
                    .Verifiable();

            // Act
            await updateCommandHandler.Handle(new UpdateCommand<ChecklistUpdateRequest>(request), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Update(It.Is<Checklist>(l => l.Id == checklistId)), Times.Once);
            RepoMock.Verify(x => x.Add(It.Is(expression)), Times.Once);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_UpdatesChecklistAndDoesntAddDefaultChecklistGivenUserWithIt()
        {
            // Arrange
            var checklists = new List<Checklist> { new Checklist { Id = 3, Name = "Untitled", UserId = userId } };
            var checklistsMock = checklists.AsQueryable().BuildMock();

            RepoMock.Setup(x => x.GetAll<Checklist>())
                    .Returns(checklistsMock.Object)
                    .Verifiable();

            // Act
            await updateCommandHandler.Handle(new UpdateCommand<ChecklistUpdateRequest>(request), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Update(It.Is<Checklist>(l => l.Id == checklistId)), Times.Once);
            RepoMock.Verify(x => x.Add(It.Is(expression)), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
        }
    }
}
