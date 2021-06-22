using System;
using System.IO;
using System.Threading.Tasks;

namespace ToDoList.SharedClientLibrary.Services
{
    public class FileConverter : IFileConverter
    {
        public async Task<byte[]> ConvertToByteArrayAsync(Stream fileStream)
        {
            _ = fileStream ?? throw new ArgumentNullException(nameof(fileStream));

            byte[] fileBytes = new byte[fileStream.Length];
            await fileStream.ReadAsync(fileBytes);

            return fileBytes;
        }
    }
}
