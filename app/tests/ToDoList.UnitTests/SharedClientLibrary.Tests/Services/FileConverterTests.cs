using System;
using System.IO;
using System.Threading.Tasks;

using FluentAssertions;

using ToDoList.SharedClientLibrary.Services;

using Xunit;

namespace SharedClientLibrary.Tests.Services
{
    public class FileConverterTests
    {
        [Fact]
        public async Task ConvertToByteArrayAsync_ReturnsByteArrayGivenStream()
        {
            // Arrange
            byte[] bytes = new byte[1520];
            new Random().NextBytes(bytes);

            FileConverter fileConverter = new();
            using var memoryStream = new MemoryStream(bytes);

            // Act
            byte[] actual = await fileConverter.ConvertToByteArrayAsync(memoryStream);

            // Assert
            actual.Should().Equal(bytes);
        }
    }
}
