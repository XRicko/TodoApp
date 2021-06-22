﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Requests.Create;

namespace ToDoList.WebApi.Services
{
    public class PhysicalFileStorage : IFileStorage
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IMediator mediator;

        public PhysicalFileStorage(IWebHostEnvironment hostEnvironment, IMediator mediatr)
        {
            webHostEnvironment = hostEnvironment;
            mediator = mediatr;
        }

        public async Task<string> SaveFileAsync(IFormFile formFile)
        {
            _ = formFile ?? throw new ArgumentNullException(nameof(formFile));

            if (formFile.Length <= 0)
                return string.Empty;

            var imageCreateRequest = MakeImageCreateRequest(formFile.FileName);

            string directoryName = Path.GetDirectoryName(imageCreateRequest.Path);
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            string existingFile = GetExistingFileInDirectory(formFile, directoryName);

            if (!string.IsNullOrWhiteSpace(existingFile))
                return existingFile;

            using var fileStream = new FileStream(imageCreateRequest.Path, FileMode.Create);
            await formFile.CopyToAsync(fileStream);

            await mediator.Send(new AddCommand<ImageCreateRequest>(imageCreateRequest));

            return imageCreateRequest.Name;
        }

        private static string GetExistingFileInDirectory(IFormFile formFile, string directoryName)
        {
            string[] files = Directory.GetFiles(directoryName);
            string existingFile = files.FirstOrDefault(x =>
            {
                using var file = File.OpenRead(x);
                return file.Length == formFile.Length;
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
