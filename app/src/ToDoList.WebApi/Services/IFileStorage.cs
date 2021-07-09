using System.Threading.Tasks;

namespace ToDoList.WebApi.Services
{
    public interface IFileStorage
    {
        Task<string> SaveFileAsync(string fileName, byte[] fileContent);
    }
}
