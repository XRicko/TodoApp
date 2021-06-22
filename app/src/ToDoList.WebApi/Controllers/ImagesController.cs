using System;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Services;
using ToDoList.WebApi.Validators;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : Base
    {
        private readonly IFileStorage fileStorage;

        public ImagesController(IMediator mediator, IFileStorage storage) : base(mediator)
        {
            fileStorage = storage;
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

            string savedFile = await fileStorage.SaveFileAsync(formFile);
            return savedFile;
        }
    }
}
