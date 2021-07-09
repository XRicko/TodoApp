using System;
using System.IO;
using System.Threading.Tasks;

namespace ToDoList.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<byte[]> ToByteArrayAsync(this Stream stream)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            byte[] bytes = new byte[stream.Length];
            await stream.ReadAsync(bytes);

            return bytes;
        }
    }
}
