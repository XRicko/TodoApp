using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Handlers.Categories;
using ToDoList.Core.Mediator.Requests.Create;

using Xunit;

namespace Core.Tests.Handlers.Generic
{
    public class AddCommandHandlerTests : HandlerBaseForTests
    {
        private readonly AddCategoryCommandHandler addCommandHandler;

        private readonly string name;
        private readonly IQueryable<Category> categories;

        public AddCommandHandlerTests() : base()
        {
            addCommandHandler = new AddCategoryCommandHandler(UnitOfWorkMock.Object, Mapper);

            name = "Misc";

            categories = new List<Category>
            {
                new Category { Id = 13, Name = name },
                new Category { Id = 63, Name = "Inro"}
            }.AsQueryable();
        }

        [Fact]
        public async Task Handle_AddsItemGivenNew()
        {
            // Arrange
            var request = new CategoryCreateRequest("New category");

            RepoMock.Setup(x => x.GetAll<Category>())
                    .Returns(categories)
                    .Verifiable();

            // Act
            await addCommandHandler.Handle(new AddCommand<CategoryCreateRequest>(request), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Add(It.Is<Category>(c => c.Name == request.Name)), Times.Once);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DoesntAddItemGivenExisting()
        {
            // Arrange
            var request = new CategoryCreateRequest(name);

            RepoMock.Setup(x => x.GetAll<Category>())
                    .Returns(categories)
                    .Verifiable();

            // Act
            await addCommandHandler.Handle(new AddCommand<CategoryCreateRequest>(request), new CancellationToken());

            // Assert
            RepoMock.Verify();
            RepoMock.Verify(x => x.Add(It.Is<Category>(x => x.Name == request.Name)), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }
    }
}
