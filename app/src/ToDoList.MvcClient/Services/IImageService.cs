using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace ToDoList.MvcClient.Services
{
    public interface IImageService
    {
        Task AddImage(IFormFile image);
    }
}