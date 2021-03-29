using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using ToDoList.MvcClient.Models;
using ToDoList.MvcClient.Services.Api;

namespace ToDoList.MvcClient.Services
{
    public class ImageAddingService : IImageAddingService
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IApiCallsService apiCallsService;

        public ImageAddingService(IWebHostEnvironment hostEnvironment, IApiCallsService service)
        {
            webHostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            apiCallsService = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task AddImageInTodoItem(TodoItemModel todoItem)
        {
            _ = todoItem ?? throw new ArgumentNullException(nameof(todoItem));

            string absolutePath = MakeAbsoluteImagePath(todoItem.Image.FileName);

            if (!File.Exists(absolutePath))
            {
                await PostImage(todoItem.Image);
                await SaveImageInFolder(todoItem.Image, absolutePath);
            }

            var image = await apiCallsService.GetItemAsync<ImageModel>("Images/GetByName/" + todoItem.Image.FileName);
            todoItem.ImageId = image.Id;
        }

        private async Task PostImage(IFormFile image)
        {
            _ = image ?? throw new ArgumentNullException(nameof(image));

            string relativePath = @"~/images/" + image.FileName;
            await apiCallsService.PostItemAsync("Images", new ImageModel { Name = image.FileName, Path = relativePath });
        }

        private string MakeAbsoluteImagePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException($"'{nameof(fileName)}' cannot be null or empty", nameof(fileName));

            string folder = Path.Combine(webHostEnvironment.WebRootPath, "images");
            string imagePath = Path.Combine(folder, fileName);

            return imagePath;
        }

        private static async Task SaveImageInFolder(IFormFile image, string imagePath)
        {
            _ = image ?? throw new ArgumentNullException(nameof(image));
            if (string.IsNullOrEmpty(imagePath))
                throw new ArgumentException($"'{nameof(imagePath)}' cannot be null or empty", nameof(imagePath));

            using var fileStream = new FileStream(imagePath, FileMode.Create);
            await image.CopyToAsync(fileStream);
        }
    }
}
