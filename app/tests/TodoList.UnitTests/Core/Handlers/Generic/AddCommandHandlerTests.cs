using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Handlers.Categories;
using ToDoList.Core.Mediator.Requests.Create;

using Xunit;

namespace Core.Handlers.Generic
{
    public class AddCommandHandlerTests : HandlerBaseForTests
    {
        private readonly AddCategoryCommandHandler addCommandHandler;

        private readonly string name;

        public AddCommandHandlerTests() : base()
        {
            addCommandHandler = new AddCategoryCommandHandler(UnitOfWorkMock.Object, Mapper);

            name = "Misc";
        }

        [Fact]
        public async Task AddsItemGivenNew()
        {
            // Arrange
            var request = new CategoryCreateRequest(name);

            RepoMock.Setup(x => x.GetAsync<Category>(request.Name))
                               .ReturnsAsync(() => null);

            // Act
            await addCommandHandler.Handle(new AddCommand<CategoryCreateRequest>(request), new CancellationToken());

            // Assert
            RepoMock.Verify(x => x.GetAsync<Category>(request.Name), Times.Once);
            RepoMock.Verify(x => x.AddAsync(It.Is<Category>(c => c.Name == request.Name)), Times.Once);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DoesntAddItemGivenExisting()
        {
            // Arrange
            var request = new CategoryCreateRequest(name);
            var entity = new Category { Name = name };

            RepoMock.Setup(x => x.GetAsync<Category>(request.Name))
                    .ReturnsAsync(entity);

            // Act
            await addCommandHandler.Handle(new AddCommand<CategoryCreateRequest>(request), new CancellationToken());

            // Assert
            RepoMock.Verify(x => x.GetAsync<Category>(request.Name), Times.Once);
            RepoMock.Verify(x => x.AddAsync(entity), Times.Never);

            UnitOfWorkMock.Verify(x => x.SaveAsync(), Times.Never);
        }
    }
}
