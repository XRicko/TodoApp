using System.IO;
using System.Threading.Tasks;

namespace ToDoList.SharedClientLibrary.Services
{
    public interface IFileConverter
    {
        Task<byte[]> ConvertToByteArrayAsync(Stream fileStream);
    }
}
