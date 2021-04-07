using System.Collections.Generic;
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
    public class GetAllQueryHandlderTests : HandlerBaseForTests
    {
        private readonly GetCategoriesQueryHandler getAllHandler;

        public GetAllQueryHandlderTests() : base()
        {
            getAllHandler = new GetCategoriesQueryHandler(UnitOfWorkMock.Object, Mapper);
        }

        [Fact]
        public async Task ReturnsListOfResponses()
        {
            // Arrange
            var entities = GetSampleEntities();
            var expected = GetSampleResponses();

            RepoMock.Setup(x => x.GetAllAsync<Category>())
                    .ReturnsAsync(entities);

            // Act
            var actual = await getAllHandler.Handle(new GetAllQuery<Category, CategoryResponse>(), new CancellationToken());

            // Assert
            Assert.Equal(expected, actual);
            RepoMock.Verify(x => x.GetAllAsync<Category>(), Times.Once);
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
