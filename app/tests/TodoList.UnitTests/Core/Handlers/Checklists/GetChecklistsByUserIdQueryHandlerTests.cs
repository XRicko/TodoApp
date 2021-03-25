using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Checklists;
using ToDoList.Core.Mediator.Queries.Checklists;
using ToDoList.Core.Mediator.Response;

using Xunit;

namespace ToDoList.UnitTests.Core.Handlers.Checklists
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
        public async Task ReturnsListOfChecklistResponsesByUser()
        {
            // Arrange
            var expected = GetSampleChecklistResponses();
            var checklists = GetSampleChecklists();

            RepoMock.Setup(x => x.GetAllAsync<Checklist>())
                    .ReturnsAsync(checklists);

            // Act
            var actual = await getChecklistsByUserIdHandler.Handle(new GetChecklistsByUserIdQuery(desirableUserId), new CancellationToken());

            // Assert
            Assert.Equal(expected, actual);
            RepoMock.Verify(x => x.GetAllAsync<Checklist>(), Times.Once);
        }

        private IEnumerable<Checklist> GetSampleChecklists()
        {
            return new List<Checklist>
            {
                new Checklist { Id = 1, Name= "Chores", UserId = desirableUserId },
                new Checklist { Id = 2, Name= "Birthday", UserId = 1 },
                new Checklist { Id = 3, Name= "Work", UserId = desirableUserId },
                new Checklist { Id = 4, Name= "Chores", UserId = 1 },
                new Checklist { Id = 5, Name= "Birthday", UserId = desirableUserId },
            };
        }

        private IEnumerable<ChecklistResponse> GetSampleChecklistResponses()
        {
            return new List<ChecklistResponse>
            {
                new(1, "Chores", desirableUserId),
                new(3, "Work", desirableUserId),
                new(5, "Birthday", desirableUserId)
            };
        }
    }
}
