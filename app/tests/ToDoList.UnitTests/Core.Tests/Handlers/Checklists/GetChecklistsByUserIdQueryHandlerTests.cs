using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MockQueryable.Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Checklists;
using ToDoList.Core.Mediator.Queries.Checklists;
using ToDoList.Core.Mediator.Response;

using Xunit;

namespace Core.Tests.Handlers.Checklists
{
    public class GetChecklistsByUserIdQueryHandlerTests : HandlerBaseForTests
    {
        private readonly GetChecklistsByUserIdQueryHandler getChecklistsByUserIdHandler;

        private readonly int desirableUserId;

        public GetChecklistsByUserIdQueryHandlerTests() : base()
        {
            getChecklistsByUserIdHandler = new GetChecklistsByUserIdQueryHandler(UnitOfWorkMock.Object, Mapper);

            desirableUserId = 3;
        }

        [Fact]
        public async Task Handle_ReturnsListOfChecklistResponsesByUser()
        {
            // Arrange
            var expected = GetSampleChecklistResponsesByUser();
            var checklists = GetSampleChecklists();

            var checklistsMock = checklists.AsQueryable().BuildMock();

            RepoMock.Setup(x => x.GetAll<Checklist>())
                    .Returns(checklistsMock.Object)
                    .Verifiable();

            // Act
            var actual = await getChecklistsByUserIdHandler.Handle(new GetChecklistsByUserIdQuery(desirableUserId), new CancellationToken());

            // Assert
            Assert.Equal(expected, actual);
            RepoMock.Verify();
        }

        private IEnumerable<Checklist> GetSampleChecklists()
        {
            return new List<Checklist>
            {
                new Checklist { Id = 1, Name= "Chores", UserId = 31 },
                new Checklist { Id = 3, Name= "Work", UserId = desirableUserId },
                new Checklist { Id = 5, Name= "Birthday", UserId = desirableUserId },
            };
        }

        private IEnumerable<ChecklistResponse> GetSampleChecklistResponsesByUser()
        {
            return new List<ChecklistResponse>
            {
                new(3, "Work", desirableUserId),
                new(5, "Birthday", desirableUserId)
            };
        }
    }
}
