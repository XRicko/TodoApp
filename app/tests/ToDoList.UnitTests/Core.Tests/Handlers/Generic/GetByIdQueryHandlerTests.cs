using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Checklists;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;

using Xunit;

namespace Core.Tests.Handlers.Generic
{
    public class GetByIdQueryHandlerTests : HandlerBaseForTests
    {
        private readonly GetChecklistByIdQueryHandler getByIdQueryHandler;

        private readonly int id;
        private readonly string name;
        private readonly int userId;

        public GetByIdQueryHandlerTests() : base()
        {
            getByIdQueryHandler = new GetChecklistByIdQueryHandler(UnitOfWorkMock.Object, Mapper);

            id = 3;
            name = "Chores";
            userId = 85;
        }

        [Fact]
        public async Task Handle_ReturnsResponseGivenProperId()
        {
            // Arrange
            var expected = new ChecklistResponse(id, name, userId);
            var entity = new Checklist { Id = id, Name = name, UserId = userId };

            RepoMock.Setup(x => x.FindByPrimaryKeysAsync<Checklist>(id))
                    .ReturnsAsync(entity);

            // Act
            var actual = await getByIdQueryHandler.Handle(new GetByIdQuery<Checklist, ChecklistResponse>(id),
                                                          new CancellationToken());

            // Assert
            // Assert
            actual.Should().Be(expected);
            RepoMock.Verify(x => x.FindByPrimaryKeysAsync<Checklist>(id), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsNullGivenInvalidId()
        {
            // Arrange
            RepoMock.Setup(x => x.FindByPrimaryKeysAsync<Checklist>(id))
                    .ReturnsAsync(() => null);

            // Act
            var actual = await getByIdQueryHandler.Handle(new GetByIdQuery<Checklist, ChecklistResponse>(id),
                                                          new CancellationToken());

            // Assert
            actual.Should().BeNull();
            RepoMock.Verify(x => x.FindByPrimaryKeysAsync<Checklist>(id), Times.Once);
        }
    }
}
