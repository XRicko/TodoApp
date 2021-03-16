using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using ToDoList.MvcClient.Models;

namespace ToDoList.MvcClient.Services
{
    public class ImageAddingService : IImageAddingService
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IApiCallsService apiCallsService;

        public ImageAddingService(IWebHostEnvironment hostEnvironment, IApiCallsService service)
        {
            webHostEnvironment = hostEnvironment;
            apiCallsService = service;
        }

        public async Task AddImageInTodoItem(TodoItemModel todoItem)
        {
            await PostImage(todoItem.Image);

            var image = await apiCallsService.GetItemAsync<ImageModel>("Images/GetByName/" + todoItem.Image.FileName);
            todoItem.ImageId = image.Id;
        }

        private async Task PostImage(IFormFile image)
        {
            string absolutePath = MakeAbsoluteImagePath(image.FileName);
            string relativePath = @"~/images/" + image.FileName;

            await SaveImageInFolder(image, absolutePath);
            await apiCallsService.PostItemAsync("Images", new ImageModel { Name = image.FileName, Path = relativePath });
        }

        private string MakeAbsoluteImagePath(string fileName)
        {
            string folder = Path.Combine(webHostEnvironment.WebRootPath, "images");
            string imagePath = Path.Combine(folder, fileName);

            return imagePath;
        }

        private static async Task SaveImageInFolder(IFormFile image, string imagePath)
        {
            using var fileStream = new FileStream(imagePath, FileMode.Create);
            await image.CopyToAsync(fileStream);
        }
    }
}
