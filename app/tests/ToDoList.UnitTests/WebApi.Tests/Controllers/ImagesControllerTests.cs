using System;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Http;

using Moq;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Controllers;
using ToDoList.WebApi.Services;

using Xunit;

namespace WebApi.Tests.Controllers
{
    public class ImagesControllerTests : ApiControllerBaseForTests
    {
        private readonly Mock<IFileStorage> fileStorageMock;
        private readonly ImagesController imagesController;

        public ImagesControllerTests() : base()
        {
            fileStorageMock = new Mock<IFileStorage>();
            imagesController = new ImagesController(MediatorMock.Object, fileStorageMock.Object);
        }

        [Fact]
        public async Task GetByName_ReturnsImageResponseGivenExistingName()
        {
            // Arrange
            string name = "image.png";
            byte[] content = new byte[525];

            new Random().NextBytes(content);

            var expected = new ImageResponse(2, name, content);

            MediatorMock.Setup(x => x.Send(It.Is<GetByNameQuery<Image, ImageResponse>>(q => q.Name == name),
                                           It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expected)
                        .Verifiable();

            // Act
            var actual = await imagesController.GetByName(name);

            // Assert
            actual.Value.Should().Be(expected);
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
            actual.Value.Should().BeNull();
            MediatorMock.Verify();
        }

        [Fact]
        public async Task Add_ReturnsAddedImageName()
        {
            // Arrange
            string imageName = "rand.jpg";
            var formFileMock = new Mock<IFormFile>();

            formFileMock.SetupGet(x => x.FileName)
                        .Returns(imageName)
                        .Verifiable();

            fileStorageMock.Setup(x => x.SaveFileAsync(formFileMock.Object))
                           .ReturnsAsync("rand.jpg")
                           .Verifiable();

            // Act
            var actual = await imagesController.Add(formFileMock.Object);

            // Assert
            actual.Value.Should().Be(imageName);

            formFileMock.Verify();
            fileStorageMock.Verify();
        }
    }
}
