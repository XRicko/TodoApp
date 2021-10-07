using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Projects;
using ToDoList.Core.Mediator.Queries.Projects;
using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel;

using Xunit;

namespace Core.Tests.Handlers.Projects
{
    public class GetProjectByNameAndUserIdQueryHandlerTests : HandlerBaseForTests
    {
        private readonly GetProjectByNameAndUserIdQueryHandler handler;

        private readonly string name;
        private readonly int userId;

        private readonly IQueryable<Project> projects;

        public GetProjectByNameAndUserIdQueryHandlerTests()
        {
            handler = new GetProjectByNameAndUserIdQueryHandler(UnitOfWorkMock.Object, Mapper);

            name = Constants.Untitled;
            userId = 58;

            projects = new List<Project>
            {
                new Project { Id = 69, Name= name, UserId = userId },
                new Project { Id = 3, Name= "Work", UserId = userId },
                new Project { Id = 5, Name= "Birthday", UserId = 2 }
            }.AsQueryable();

            RepoMock.Setup(x => x.GetAll<Project>())
                    .Returns(projects)
                    .Verifiable();
        }

        [Fact]
        public async Task Handle_ReturnsNullGivenInvalidValues()
        {
            // Act
            var actual = await handler.Handle(new GetProjectByNameAndUserIdQuery(name, 420), new CancellationToken());

            // Assert
            actual.Should().BeNull();
            RepoMock.Verify();
        }

        [Fact]
        public async Task Handle_ReturnsProjectResponeByNameAndUserIdGivenProperValues()
        {
            // Arrange
            ProjectResponse expected = new(69, name, userId);

            // Act
            var actual = await handler.Handle(new GetProjectByNameAndUserIdQuery(name, userId), new CancellationToken());

            // Assert
            actual.Should().Be(expected);
            RepoMock.Verify();
        }
    }
}
