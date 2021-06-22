using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace ToDoList.WebApi.Services
{
    public interface IFileStorage
    {
        Task<string> SaveFileAsync(IFormFile formFile);
    }
}
