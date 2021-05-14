using System.Threading;
using System.Threading.Tasks;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Controllers;

using Xunit;

namespace WebApi.Tests.Controllers
{
    public class ImagesControllerTests : ApiControllerBaseForTests
    {
        private readonly ImagesController imagesController;

        public ImagesControllerTests() : base()
        {
            imagesController = new ImagesController(MediatorMock.Object);
        }

        [Fact]
        public async Task GetByName_ReturnsImageResponseGivenExistingName()
        {
            // Arrange
            string name = "image.png";
            var expected = new ImageResponse(2, name, "C:\\users\\image.png");

            MediatorMock.Setup(x => x.Send(It.Is<GetByNameQuery<Image, ImageResponse>>(q => q.Name == name),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected)
                        .Verifiable();

            // Act
            var actual = await imagesController.GetByName(name);

            // Assert
            Assert.Equal(expected, actual);
            MediatorMock.Verify();
        }

        [Fact]
        public async Task GetByName_ReturnsNullGivenInvalidName()
        {
            // Arrange
            string name = "wrong_name";

            MediatorMock.Setup(x => x.Send(It.Is<GetByNameQuery<Image, ImageResponse>>(q => q.Name == name),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => null)
                        .Verifiable();

            // Act
            var actual = await imagesController.GetByName(name);

            // Assert
            Assert.Null(actual);
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Add_SendsRequest()
        {
            // Arrange
            var createRequest = new ImageCreateRequest("Essential", "C:\\");

            // Act
            await imagesController.Add(createRequest);

            // Assert
            MediatorMock.Verify(x => x.Send(It.Is<AddCommand<ImageCreateRequest>>(q => q.Request == createRequest),
                                            It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
