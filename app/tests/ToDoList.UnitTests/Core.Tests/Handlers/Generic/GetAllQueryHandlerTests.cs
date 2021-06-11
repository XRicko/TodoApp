using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using MockQueryable.Moq;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Categories;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;

using Xunit;

namespace Core.Tests.Handlers.Generic
{
    public class GetAllQueryHandlerTests : HandlerBaseForTests
    {
        private readonly GetCategoriesQueryHandler getAllHandler;

        public GetAllQueryHandlerTests() : base()
        {
            getAllHandler = new GetCategoriesQueryHandler(UnitOfWorkMock.Object, Mapper);
        }

        [Fact]
        public async Task Handle_ReturnsListOfResponses()
        {
            // Arrange
            var entities = GetSampleEntities();
            var expected = GetSampleResponses();

            var entitiesMock = entities.AsQueryable().BuildMock();

            RepoMock.Setup(x => x.GetAll<Category>())
                    .Returns(entitiesMock.Object);

            // Act
            var actual = await getAllHandler.Handle(new GetAllQuery<Category, CategoryResponse>(), new CancellationToken());

            // Assert
            actual.Should().Equal(expected);
            RepoMock.Verify(x => x.GetAll<Category>(), Times.Once);
        }

        private static IEnumerable<Category> GetSampleEntities()
        {
            return new List<Category>
            {
                new() { Name = "Important"},
                new() { Name = "Unimportant" },
                new() { Name = "For fun" }
            };
        }

        private static IEnumerable<CategoryResponse> GetSampleResponses()
        {
            return new List<CategoryResponse>
            {
                new(0, "Important"),
                new(0, "Unimportant"),
                new(0, "For fun")
            };
        }
    }
}
