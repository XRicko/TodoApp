using System;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.Extensions;
using ToDoList.WebApi.Services;
using ToDoList.WebApi.Validators;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : Base
    {
        private readonly IFileStorage fileStorage;
        private readonly IImageMinifier imageResizer;

        public ImagesController(IMediator mediator, IFileStorage storage, IImageMinifier resizer) : base(mediator)
        {
            fileStorage = storage ?? throw new ArgumentNullException(nameof(storage));
            imageResizer = resizer ?? throw new ArgumentNullException(nameof(resizer));
        }

        [HttpGet]
        [Route("[action]/{name}")]
        public async Task<ActionResult<ImageResponse>> GetByName(string name) =>
            await Mediator.Send(new GetByNameQuery<Image, ImageResponse>(name));

        [HttpPost]
        public async Task<ActionResult<string>> Add(IFormFile formFile)
        {
            _ = formFile ?? throw new ArgumentNullException(nameof(formFile));

            var result = new FormFileValidator().Validate(formFile);

            if (!result.IsValid)
                return BadRequest(result.Errors);

            using var stream = formFile.OpenReadStream();
            byte[] original = await stream.ToByteArrayAsync();

            byte[] minified = await imageResizer.ResizeAsync(original);

            string savedFile = await fileStorage.SaveFileAsync(formFile.FileName, minified);
            return savedFile;
        }
    }
}
