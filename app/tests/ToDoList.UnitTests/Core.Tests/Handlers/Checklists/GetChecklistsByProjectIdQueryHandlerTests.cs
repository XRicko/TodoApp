using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Checklists;
using ToDoList.Core.Mediator.Queries.Checklists;
using ToDoList.Core.Mediator.Response;

using Xunit;

namespace Core.Tests.Handlers.Checklists
{
    public class GetChecklistsByProjectIdQueryHandlerTests : HandlerBaseForTests
    {
        private readonly GetChecklistsByProjectIdQueryHandler getChecklistsByUserIdHandler;

        public GetChecklistsByProjectIdQueryHandlerTests() : base()
        {
            getChecklistsByUserIdHandler = new GetChecklistsByProjectIdQueryHandler(UnitOfWorkMock.Object, Mapper);
        }

        [Fact]
        public async Task Handle_ReturnsListOfChecklistResponsesByProject()
        {
            // Arrange
            int desirableProjectId = 3;

            var expected = new List<ChecklistResponse>
            {
                new(3, "Work", desirableProjectId),
                new(5, "Birthday", desirableProjectId)
            };

            var checklists = new List<Checklist>
            {
                new Checklist { Id = 1, Name= "Chores", ProjectId = 31 },
                new Checklist { Id = 3, Name= "Work", ProjectId = desirableProjectId },
                new Checklist { Id = 5, Name= "Birthday", ProjectId = desirableProjectId },
            }.AsQueryable();

            RepoMock.Setup(x => x.GetAll<Checklist>())
                    .Returns(checklists)
                    .Verifiable();

            // Act
            var actual = await getChecklistsByUserIdHandler.Handle(new GetChecklistsByProjectIdQuery(desirableProjectId), new CancellationToken());

            // Assert
            actual.Should().Equal(expected);
            RepoMock.Verify();
        }
    }
}
