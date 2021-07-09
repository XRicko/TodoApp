using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Hosting;

using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Requests.Create;

namespace ToDoList.WebApi.Services
{
    public class PhysicalFileStorage : IFileStorage
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IMediator mediator;
        private readonly IFileSystem fileSystem;

        public PhysicalFileStorage(IWebHostEnvironment hostEnvironment, IMediator mediatr, IFileSystem system)
        {
            webHostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            mediator = mediatr ?? throw new ArgumentNullException(nameof(mediatr));
            fileSystem = system ?? throw new ArgumentNullException(nameof(system));
        }

        public async Task<string> SaveFileAsync(string fileName, byte[] fileContent)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException($"'{nameof(fileName)}' cannot be null or whitespace.", nameof(fileName));
            _ = fileContent ?? throw new ArgumentNullException(nameof(fileContent));

            if (fileContent.Length <= 0)
                return string.Empty;

            var imageCreateRequest = MakeImageCreateRequest(fileName);

            string directoryName = Path.GetDirectoryName(imageCreateRequest.Path);
            if (!fileSystem.Directory.Exists(directoryName))
                fileSystem.Directory.CreateDirectory(directoryName);

            string existingFile = GetExistingFileInDirectory(fileContent.LongLength, directoryName);

            if (!string.IsNullOrWhiteSpace(existingFile))
                return existingFile;

            using var fileStream = fileSystem.FileStream.Create(imageCreateRequest.Path, FileMode.Create);
            await fileStream.WriteAsync(fileContent);

            await mediator.Send(new AddCommand<ImageCreateRequest>(imageCreateRequest));

            return imageCreateRequest.Name;
        }

        private string GetExistingFileInDirectory(long fileLength, string directoryName)
        {
            string[] files = fileSystem.Directory.GetFiles(directoryName);
            string existingFile = files.FirstOrDefault(x =>
            {
                using var file = fileSystem.File.OpenRead(x);
                return file.Length == fileLength;
            });

            return Path.GetFileName(existingFile);
        }

        private ImageCreateRequest MakeImageCreateRequest(string fileName)
        {
            string fileNameWithExtension = GetFileNameWithExtension(fileName);
            string relativePath = GetRelativePath(fileNameWithExtension);

            ImageCreateRequest imageCreateRequest = new(fileNameWithExtension, relativePath);

            return imageCreateRequest;
        }

        private static string GetFileNameWithExtension(string fileName)
        {
            string randomFileName = Path.GetRandomFileName();

            string extension = Path.GetExtension(fileName);
            string fileNameWithExtension = Path.ChangeExtension(randomFileName, extension);

            return fileNameWithExtension;
        }

        private string GetRelativePath(string fileNameWithExtension)
        {
            string folder = Path.Combine(webHostEnvironment.ContentRootPath, "Images");

            string absolutePath = Path.Combine(folder, fileNameWithExtension);
            string relativePath = Path.GetRelativePath(webHostEnvironment.ContentRootPath, absolutePath);

            return relativePath;
        }
    }
}
