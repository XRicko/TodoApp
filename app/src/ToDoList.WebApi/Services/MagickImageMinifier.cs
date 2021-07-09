using System.IO;
using System.Threading.Tasks;

using ImageMagick;

using ToDoList.Extensions;

namespace ToDoList.WebApi.Services
{
    public class MagickImageMinifier : IImageMinifier
    {
        public Task<byte[]> ResizeAsync(byte[] originalContent, int width = 600, int height = 340)
        {
            using var stream = new MemoryStream();
            using var image = new MagickImage(originalContent);

            var size = new MagickGeometry(width, height);

            image.Resize(size);
            image.Write(stream);

            stream.Position = 0;

            return stream.ToByteArrayAsync();
        }
    }
}
