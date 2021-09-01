using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

using FluentAssertions;

using ToDoList.WebApi.Services;

using Xunit;

namespace WebApi.Tests.Services
{
    public class MagickImageMinifierTests
    {
        private readonly MagickImageMinifier minifier;

        public MagickImageMinifierTests()
        {
            minifier = new MagickImageMinifier();
        }

        [Fact]
        public async Task ResizeAsync_ReturnsResizedImage()
        {
            // Arrange
            var stream = new MemoryStream();

            Bitmap bmpImage = new(600, 400);
            bmpImage.Save(stream, ImageFormat.Jpeg);

            // Act
            byte[] rezised = await minifier.ResizeAsync(stream.ToArray());

            // Assert
            rezised.LongLength.Should().BeLessThan(stream.Length);
        }
    }
}
