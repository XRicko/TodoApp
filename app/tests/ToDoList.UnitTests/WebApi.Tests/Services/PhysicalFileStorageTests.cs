using System;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using MediatR;

using Microsoft.AspNetCore.Hosting;

using Moq;

using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.WebApi.Services;

using Xunit;

namespace WebApi.Tests.Services
{
    public class PhysicalFileStorageTests
    {
        private readonly PhysicalFileStorage physicalFileStorage;

        private readonly Mock<IWebHostEnvironment> webHostEnvironmentMock;
        private readonly Mock<IMediator> mediatorMock;
        private readonly MockFileSystem fileSystemMock;

        private readonly string fileName;

        public PhysicalFileStorageTests()
        {
            webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            mediatorMock = new Mock<IMediator>();

            fileName = "ranje.jpg";
            fileSystemMock = new MockFileSystem();

            physicalFileStorage = new PhysicalFileStorage(webHostEnvironmentMock.Object, mediatorMock.Object, fileSystemMock);
        }

        [Fact]
        public async Task SaveFileAsync_ReturnsEmptyStringGivenContentWithInvalidLenght()
        {
            // Arrange
            byte[] bytes = Array.Empty<byte>();

            // Act
            string actual = await physicalFileStorage.SaveFileAsync(fileName, bytes);

            // Assert
            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task SaveFileAsync_ReturnsNameOfExistingFileGivenFileWithSameLenght()
        {
            // Arrange
            string path = $"Images\\{fileName}";
            fileSystemMock.AddFile(path, new MockFileData("some data"));

            using var stream = fileSystemMock.File.OpenRead(path);
            byte[] bytes = new byte[stream.Length];

            webHostEnvironmentMock.SetupGet(x => x.ContentRootPath)
                                  .Returns("c:\\")
                                  .Verifiable();

            // Act
            string actual = await physicalFileStorage.SaveFileAsync(fileName, bytes);

            // Assert
            actual.Should().Be(fileName);
            webHostEnvironmentMock.Verify();
        }

        [Fact]
        public async Task SaveFileAsync_ReturnsNameOfNewlyAddedFileGivenFileWithUniqueLenght()
        {
            // Arrange
            int length = 789;

            byte[] bytes = new byte[length];
            new Random().NextBytes(bytes);

            webHostEnvironmentMock.SetupGet(x => x.ContentRootPath)
                                  .Returns("c:\\")
                                  .Verifiable();

            // Act
            string actual = await physicalFileStorage.SaveFileAsync(fileName, bytes);

            // Assert
            actual.Should().NotBe(fileName);

            fileSystemMock.AllDirectories.Should().Contain(x => x.Contains("Images"));
            fileSystemMock.AllFiles.Should().Contain(x => fileSystemMock.Path.GetFileName(x) == actual);

            webHostEnvironmentMock.Verify();
            mediatorMock.Verify(x => x.Send(It.Is<AddCommand<ImageCreateRequest>>(r => r.Request.Path.Contains("Images")),
                                            It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
