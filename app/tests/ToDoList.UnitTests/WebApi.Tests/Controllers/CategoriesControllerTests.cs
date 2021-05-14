using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Controllers;

using Xunit;

namespace ToDoList.UnitTests.WebApi.Controllers
{
    public class CategoriesControllerTests : ApiControllerBaseForTests
    {
        private readonly IDistributedCache cache;
        private readonly CategoriesController categoriesController;

        public CategoriesControllerTests() : base()
        {
            var opts = Options.Create(new MemoryDistributedCacheOptions());
            cache = new MemoryDistributedCache(opts);

            categoriesController = new CategoriesController(MediatorMock.Object, cache);
        }

        [Fact]
        public async Task Get_ReturnsNewListOfCategoryResponses()
        {
            // Arrange
            var expected = new List<CategoryResponse>
            {
                new(1, "Important"),
                new(2, "Unimportant"),
                new(3, "Uncategorized")
            };

            MediatorMock.Setup(x => x.Send(It.IsAny<GetAllQuery<Category, CategoryResponse>>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected)
                        .Verifiable();

            // Act
            var actual = await categoriesController.Get();

            // Assert
            Assert.Equal(expected, actual);
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Get_ReturnsListOfCategoryResponsesFromCache()
        {
            // Arrange
            var expected = new List<CategoryResponse>
            {
                new(1, "Important"),
                new(2, "Unimportant"),
                new(3, "Uncategorized")
            };

            string recordKey = "Categories";
            cache.SetString(recordKey, JsonSerializer.Serialize(expected));

            // Act
            var actual = await categoriesController.Get();

            // Assert
            Assert.Equal(expected, actual);
            MediatorMock.Verify(x => x.Send(It.IsAny<GetAllQuery<Category, CategoryResponse>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetByName_ReturnsCategoryResponseGivenExistingName()
        {
            // Arrange
            string name = "Imp";
            var expected = new CategoryResponse(0, name);

            MediatorMock.Setup(x => x.Send(It.Is<GetByNameQuery<Category, CategoryResponse>>(q => q.Name == name), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected)
                        .Verifiable();

            // Act
            var actual = await categoriesController.GetByName(name);

            // Assert
            Assert.Equal(expected, actual);
            MediatorMock.Verify();
        }

        [Fact]
        public async Task GetByName_ReturnsNullGivenInvalidName()
        {
            // Arrange
            string invalidName = "invalid_name";

            MediatorMock.Setup(x => x.Send(It.Is<GetByNameQuery<Category, CategoryResponse>>(q => q.Name == invalidName), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => null)
                        .Verifiable();

            // Act
            var actual = await categoriesController.GetByName(invalidName);

            // Assert
            Assert.Null(actual);
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Add_SendsRequest()
        {
            // Arrange
            var createRequest = new CategoryCreateRequest("Essential");

            MediatorMock.Setup(x => x.Send(It.Is<AddCommand<CategoryCreateRequest>>(q => q.Request == createRequest), It.IsAny<CancellationToken>()))
                        .Verifiable();

            // Act
            await categoriesController.Add(createRequest);

            // Assert
            MediatorMock.Verify();
        }
    }
}
