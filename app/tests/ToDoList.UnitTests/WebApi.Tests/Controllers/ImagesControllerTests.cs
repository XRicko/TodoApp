using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Http;

using Moq;

using ToDoList.Core;
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
        private readonly Mock<IImageMinifier> imageMinifierMock;

        private readonly ImagesController imagesController;

        public ImagesControllerTests() : base()
        {
            fileStorageMock = new Mock<IFileStorage>();
            imageMinifierMock = new Mock<IImageMinifier>();

            imagesController = new ImagesController(MediatorMock.Object, fileStorageMock.Object, imageMinifierMock.Object);
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

            byte[] original = new byte[1234567];
            byte[] minified = new byte[12345];

            new Random().NextBytes(original);
            new Random().NextBytes(minified);

            IFormFile formFile = new FormFile(new MemoryStream(original), 0, original.LongLength, "name", imageName);

            imageMinifierMock.Setup(x => x.ResizeAsync(original, 600, 340))
                             .ReturnsAsync(minified)
                             .Verifiable();

            fileStorageMock.Setup(x => x.SaveFileAsync(imageName, minified))
                           .ReturnsAsync("rand.jpg")
                           .Verifiable();

            // Act
            var actual = await imagesController.Add(formFile);

            // Assert
            actual.Value.Should().Be(imageName);

            imageMinifierMock.Verify();
            fileStorageMock.Verify();
        }
    }
}
