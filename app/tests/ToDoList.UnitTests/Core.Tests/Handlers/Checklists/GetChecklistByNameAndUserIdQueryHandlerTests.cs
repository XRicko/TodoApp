using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using MockQueryable.Moq;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Checklists;
using ToDoList.Core.Mediator.Queries.Checklists;
using ToDoList.Core.Mediator.Response;

using Xunit;

namespace Core.Tests.Handlers.Checklists
{
    public class GetChecklistByNameAndUserIdQueryHandlerTests : HandlerBaseForTests
    {
        private readonly GetChecklistByNameAndUserIdQueryHandler handler;

        private readonly string name;
        private readonly int userId;

        private readonly Mock<IQueryable<Checklist>> checklistsMock;

        public GetChecklistByNameAndUserIdQueryHandlerTests()
        {
            handler = new GetChecklistByNameAndUserIdQueryHandler(UnitOfWorkMock.Object, Mapper);

            name = "Some name";
            userId = 13;

            var checklists = GetSampleChecklists();
            checklistsMock = checklists.AsQueryable().BuildMock();

            RepoMock.Setup(x => x.GetAll<Checklist>())
                    .Returns(checklistsMock.Object)
                    .Verifiable();
        }

        [Fact]
        public async Task Handle_ReturnsNullGivenInvalidValues()
        {
            // Act
            var actual = await handler.Handle(new GetChecklistByNameAndUserIdQuery(name, 420), new CancellationToken());

            // Assert
            actual.Should().BeNull();
            RepoMock.Verify();
        }

        [Fact]
        public async Task Handle_ReturnsChecklistResponeByNameAndUserIdGivenProperValues()
        {
            // Arrange
            ChecklistResponse expected = new(69, name, userId);

            // Act
            var actual = await handler.Handle(new GetChecklistByNameAndUserIdQuery(name, userId), new CancellationToken());

            // Assert
            actual.Should().Be(expected);
            RepoMock.Verify();
        }

        private IEnumerable<Checklist> GetSampleChecklists()
        {
            return new List<Checklist>
            {
                new Checklist { Id = 69, Name= name, UserId = userId },
                new Checklist { Id = 3, Name= "Work", UserId = userId },
                new Checklist { Id = 5, Name= "Birthday", UserId = 2 }
            };
        }
    }
}
