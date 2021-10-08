using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Handlers.Projects;

using Xunit;

namespace Core.Tests.Handlers.Projects
{
    public class RemoveProjectCommandHandlerTests : HandlerBaseForTests
    {
        private readonly RemoveProjectByIdCommandHandler handler;

        private readonly int id;
        private readonly int firstChecklistId;
        private readonly int secondChecklistId;

        private readonly IQueryable<Project> projects;

        public RemoveProjectCommandHandlerTests()
        {
            handler = new RemoveProjectByIdCommandHandler(UnitOfWorkMock.Object, Mapper);

            id = 42;
            firstChecklistId = 13;
            secondChecklistId = 5;

            projects = GetSampleProjects().AsQueryable();
        }

        [Fact]
        public async Task Handle_DeletesProjectsWithChecklistsGivenExistingWithProperId()
        {
            // Arrange
            var checklists = GetSampleChecklists().AsQueryable();
            int[] checklistsId = new[] { firstChecklistId, secondChecklistId };

            var projectToDelete = projects.SingleOrDefault(x => x.Id == id);

            RepoMock.Setup(x => x.GetAll<Project>())
                    .Returns(projects)
                    .Verifiable();


            RepoMock.Setup(x => x.GetAll<Checklist>())
                    .Returns(checklists)
                    .Verifiable();

            // Act
            await handler.Handle(new RemoveByIdCommand<Project>(id), new CancellationToken());

            // Arrange
            RepoMock.Verify();

            RepoMock.Verify(x => x.Remove(It.Is<Project>(x => x.Id == id)), Times.Once);
            RepoMock.Verify(x => x.Remove(It.Is<Checklist>(x => checklistsId.SequenceEqual(projectToDelete.Checklists.Select(c => c.Id)))),
                            Times.Exactly(projectToDelete.Checklists.Count));
            RepoMock.Verify(x => x.Remove(It.Is<TodoItem>(x => checklistsId.Contains(x.ChecklistId))),
                            Times.Exactly(projectToDelete.Checklists.Select(x => x.TodoItems.Count).Sum()));

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DoesntDeleteProjectGivenInvalidId()
        {
            // Arrange
            int invalidId = 13456;

            RepoMock.Setup(x => x.GetAll<Project>())
                    .Returns(projects)
                    .Verifiable();

            // Act
            await handler.Handle(new RemoveByIdCommand<Project>(invalidId), new CancellationToken());

            // Assert
            RepoMock.Verify();

            RepoMock.Verify(x => x.Remove(It.IsAny<Project>()), Times.Never);
            RepoMock.Verify(x => x.Remove(It.IsAny<Checklist>()), Times.Never());
            RepoMock.Verify(x => x.Remove(It.IsAny<TodoItem>()), Times.Never());

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }

        private List<Project> GetSampleProjects()
        {
            return new List<Project>
            {
                new Project
                {
                    Id = id,
                    Name = "Something",
                    Checklists = GetSampleChecklists()
                },
                new Project
                {
                    Id = 58,
                    Name = "Other"
                }
            };
        }

        private List<Checklist> GetSampleChecklists()
        {
            return new List<Checklist>
            {
                new Checklist
                {
                    Id = firstChecklistId,
                    Name = "Birthday",
                    ProjectId = id,

                    TodoItems = new List<TodoItem>
                    {
                        new TodoItem { Name = "Invite friends", ChecklistId = firstChecklistId },
                        new TodoItem { Name = "Prepare a party", ChecklistId = firstChecklistId },
                        new TodoItem { Name = "Buy everyting needed", ChecklistId = firstChecklistId }
                    }
                },
                new Checklist
                {
                    Id = secondChecklistId,
                    Name = "Chores",
                    ProjectId = id,

                    TodoItems = new List<TodoItem>
                    {
                        new TodoItem { Name = "Clean a room", ChecklistId = secondChecklistId },
                    }
                }
            };
        }
    }
}
