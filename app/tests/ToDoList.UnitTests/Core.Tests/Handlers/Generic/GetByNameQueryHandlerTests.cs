using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Categories;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;

using Xunit;

namespace Core.Tests.Handlers.Generic
{
    public class GetByNameQueryHandlerTests : HandlerBaseForTests
    {
        private readonly GetCategoryByNameQueryHandler getByNameHandler;

        private readonly string name;

        public GetByNameQueryHandlerTests() : base()
        {
            getByNameHandler = new GetCategoryByNameQueryHandler(UnitOfWorkMock.Object, Mapper);

            name = "Misc";
        }

        [Fact]
        public async Task Handle_ReturnsResponseGivenProperName()
        {
            // Arrange
            var expected = new CategoryResponse(5, name);
            var entity = new Category { Id = 5, Name = name };

            RepoMock.Setup(x => x.GetAsync<Category>(name))
                    .ReturnsAsync(entity);

            // Act
            var actual = await getByNameHandler.Handle(new GetByNameQuery<Category, CategoryResponse>(name), new CancellationToken());

            // Assert
            Assert.Equal(expected, actual);
            RepoMock.Verify(x => x.GetAsync<Category>(name), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsNullGivenInvalidName()
        {
            // Arrange
            RepoMock.Setup(x => x.GetAsync<Category>(name))
                    .ReturnsAsync(() => null);

            // Act
            var actual = await getByNameHandler.Handle(new GetByNameQuery<Category, CategoryResponse>(name), new CancellationToken());

            // Assert
            Assert.Null(actual);
            RepoMock.Verify(x => x.GetAsync<Category>(name), Times.Once);
        }
    }
}
