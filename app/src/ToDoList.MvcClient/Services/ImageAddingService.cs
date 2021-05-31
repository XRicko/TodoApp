using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using ToDoList.MvcClient.Models;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.MvcClient.Services
{
    [ExcludeFromCodeCoverage]
    public class ImageAddingService : IImageAddingService
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IApiInvoker apiInvoker;

        public ImageAddingService(IWebHostEnvironment hostEnvironment, IApiInvoker invoker)
        {
            webHostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            apiInvoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
        }

        public async Task AddImageInTodoItemAsync(TodoItemModelWithFile todoItem)
        {
            _ = todoItem ?? throw new ArgumentNullException(nameof(todoItem));
            _ = todoItem.Image ?? throw new ArgumentNullException(nameof(todoItem.Image));

            string absolutePath = MakeAbsoluteImagePath(todoItem.Image.FileName);

            if (!File.Exists(absolutePath))
            {
                await PostImage(todoItem.Image);
                await SaveImageInFolder(todoItem.Image, absolutePath);
            }

            var image = await apiInvoker.GetItemAsync<ImageModel>("Images/GetByName/" + todoItem.Image.FileName);
            todoItem.ImageId = image.Id;
        }

        private async Task PostImage(IFormFile image)
        {
            _ = image ?? throw new ArgumentNullException(nameof(image));

            string relativePath = @"~/images/" + image.FileName;
            await apiInvoker.PostItemAsync("Images", new ImageModel { Name = image.FileName, Path = relativePath });
        }

        private string MakeAbsoluteImagePath(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException($"'{nameof(fileName)}' cannot be null or whitespace", nameof(fileName));

            string folder = Path.Combine(webHostEnvironment.WebRootPath, "images");
            string imagePath = Path.Combine(folder, fileName);

            return imagePath;
        }

        private static async Task SaveImageInFolder(IFormFile image, string imagePath)
        {
            _ = image ?? throw new ArgumentNullException(nameof(image));
            if (string.IsNullOrWhiteSpace(imagePath))
                throw new ArgumentException($"'{nameof(imagePath)}' cannot be null or whitespace", nameof(imagePath));

            using var fileStream = new FileStream(imagePath, FileMode.Create);
            await image.CopyToAsync(fileStream);
        }
    }
}
