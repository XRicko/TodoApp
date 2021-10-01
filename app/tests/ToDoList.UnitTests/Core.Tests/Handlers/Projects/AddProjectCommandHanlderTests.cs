using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Handlers.Projects;
using ToDoList.Core.Mediator.Requests.Create;

using Xunit;

namespace Core.Tests.Handlers.Projects
{
    public class AddProjectCommandHanlderTests : HandlerBaseForTests
    {
        private readonly AddProjectCommandHandler handler;

        private readonly string name;

        public AddProjectCommandHanlderTests()
        {
            handler = new AddProjectCommandHandler(UnitOfWorkMock.Object, Mapper);

            name = "Something";
        }

        [Fact]
        public async Task Handle_AddsProjectGivenNew()
        {
            // Arrange
            int userId = 1;

            var newProject = new ProjectCreateRequest(name, userId);

            var projects = new List<Project> { new Project { Id = 12, Name = "Old" } }.AsQueryable();

            RepoMock.Setup(x => x.GetAll<Project>())
                    .Returns(projects)
                    .Verifiable();

            // Act
            await handler.Handle(new AddCommand<ProjectCreateRequest>(newProject), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Add(It.Is<Project>(e => e.Name == name && e.UserId == userId)), Times.Once);
            RepoMock.Verify(x => x.Add(It.Is<Checklist>(l => l.Name == "Untitled")), Times.Once);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_DoesntAddProjectGivenExisting()
        {
            // Arrange
            int userId = 2;

            var existingProject = new Project { Id = 3, Name = name, UserId = userId };
            var request = new ProjectCreateRequest(name, userId);

            var projects = new List<Project> { existingProject }.AsQueryable();

            RepoMock.Setup(x => x.GetAll<Project>())
                    .Returns(projects)
                    .Verifiable();

            // Act
            await handler.Handle(new AddCommand<ProjectCreateRequest>(request), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Add(It.Is<Checklist>(e => e.Name == name && e.ProjectId == userId)), Times.Never);
            RepoMock.Verify(x => x.Add(It.Is<Checklist>(l => l.Name == "Untitled")), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }
    }
}
