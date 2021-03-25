
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Controllers;

using Xunit;

namespace ToDoList.UnitTests.WebApi.Controllers
{
    public class CategoriesControllerTests : ControllerBaseForTests
    {
        private readonly CategoriesController categoriesController;

        public CategoriesControllerTests() : base()
        {
            categoriesController = new CategoriesController(MediatorMock.Object);
        }

        [Fact]
        public async Task Get_ReturnsListOfCategoryResponses()
        {
            // Arrange
            var expected = new List<CategoryResponse>
            {
                new(1, "Important"),
                new(2, "Unimportant"),
                new(3, "Uncategorized")
            };

            MediatorMock.Setup(x => x.Send(It.IsAny<GetAllQuery<Category, CategoryResponse>>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected);

            // Act
            var actual = await categoriesController.Get();

            // Assert
            Assert.Equal(expected, actual);
            MediatorMock.Verify();
        }

        [Fact]
        public async Task GetByName_ReturnsCategoryResponseGivenExistingName()
        {
            // Arrange
            string name = "Imp";
            var expected = new CategoryResponse(0, name);

            MediatorMock.Setup(x => x.Send(It.Is<GetByNameQuery<Category, CategoryResponse>>(q => q.Name == name), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected);

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
                      .ReturnsAsync(() => null);

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

            MediatorMock.Setup(x => x.Send(It.Is<AddCommand<CategoryCreateRequest>>(q => q.Request == createRequest), It.IsAny<CancellationToken>()));

            // Act
            await categoriesController.Add(createRequest);

            // Assert
            MediatorMock.Verify();
        }
    }
}
