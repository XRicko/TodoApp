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
    public class GetChecklistByNameAndProjectIdQueryHandlerTests : HandlerBaseForTests
    {
        private readonly GetChecklistByNameAndProjectIdQueryHandler handler;

        private readonly string name;
        private readonly int projectId;

        private readonly IQueryable<Checklist> checklists;

        public GetChecklistByNameAndProjectIdQueryHandlerTests()
        {
            handler = new GetChecklistByNameAndProjectIdQueryHandler(UnitOfWorkMock.Object, Mapper);

            name = "Some name";
            projectId = 13;

            checklists = new List<Checklist>
            {
                new Checklist { Id = 69, Name= name, ProjectId = projectId },
                new Checklist { Id = 3, Name= "Work", ProjectId = projectId },
                new Checklist { Id = 5, Name= "Birthday", ProjectId = 2 }
            }.AsQueryable();

            RepoMock.Setup(x => x.GetAll<Checklist>())
                    .Returns(checklists)
                    .Verifiable();
        }

        [Fact]
        public async Task Handle_ReturnsNullGivenInvalidValues()
        {
            // Act
            var actual = await handler.Handle(new GetChecklistByNameAndProjectIdQuery(name, 420), new CancellationToken());

            // Assert
            actual.Should().BeNull();
            RepoMock.Verify();
        }

        [Fact]
        public async Task Handle_ReturnsChecklistResponeByNameAndUserIdGivenProperValues()
        {
            // Arrange
            ChecklistResponse expected = new(69, name, projectId);

            // Act
            var actual = await handler.Handle(new GetChecklistByNameAndProjectIdQuery(name, projectId), new CancellationToken());

            // Assert
            actual.Should().Be(expected);
            RepoMock.Verify();
        }
    }
}
