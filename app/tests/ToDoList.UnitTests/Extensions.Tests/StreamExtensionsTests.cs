using System;
using System.IO;
using System.Threading.Tasks;

using FluentAssertions;

using ToDoList.Extensions;

using Xunit;

namespace Extensions.Tests
{
    public class StreamExtensionsTests
    {
        [Fact]
        public async Task ToByteArrayAsync_ReturnsByteArray()
        {
            // Arrange
            byte[] bytes = new byte[7854];
            new Random().NextBytes(bytes);

            using var stream = new MemoryStream(bytes);

            // Act
            byte[] actual = await stream.ToByteArrayAsync();

            // Assert
            actual.Should().Equal(bytes);
        }
    }
}
