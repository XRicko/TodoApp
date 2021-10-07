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
    public class GetProjectsByUserIdQueryHandlerTests : HandlerBaseForTests
    {
        private readonly GetProjectsByUserIdQueryHandler handler;


        public GetProjectsByUserIdQueryHandlerTests()
        {
            handler = new GetProjectsByUserIdQueryHandler(UnitOfWorkMock.Object, Mapper);
        }

        [Fact]
        public async Task Handle_ReturnsListOfProjectsByUser()
        {
            // Arrange
            int desirableUserId = 3;

            var expected = new List<ProjectResponse>
            {
                new(3, Constants.Untitled, desirableUserId),
                new(5, "Draft", desirableUserId)
            };

            var projects = new List<Project>
            {
                new Project { Id = 1, Name = Constants.Untitled, UserId = 31 },
                new Project { Id = 3, Name = Constants.Untitled, UserId = desirableUserId },
                new Project { Id = 5, Name = "Draft", UserId = desirableUserId },
            }.AsQueryable();

            RepoMock.Setup(x => x.GetAll<Project>())
                    .Returns(projects)
                    .Verifiable();

            // Act
            var actual = await handler.Handle(new GetProjectsByUserIdQuery(desirableUserId), new CancellationToken());

            // Assert
            actual.Should().Equal(expected);
            RepoMock.Verify();
        }
    }
}
