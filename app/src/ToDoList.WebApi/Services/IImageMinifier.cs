using System.Threading.Tasks;

namespace ToDoList.WebApi.Services
{
    public interface IImageMinifier
    {
        public Task<byte[]> ResizeAsync(byte[] originalContent, int width = 600, int height = 340);
    }
}
