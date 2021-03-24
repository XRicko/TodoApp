using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Response;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : Base
    {
        public ImagesController(IMediator mediator) : base(mediator)
        {

        }

        [HttpGet]
        [Route("[action]/{name}")]
        public async Task<ImageResponse> GetByName(string name) =>
          await Mediator.Send(new GetByNameQuery<Image, ImageResponse>(name));

        [HttpPost]
        public async Task Add([FromBody] ImageCreateRequest createRequest) =>
            await Mediator.Send(new AddCommand<ImageCreateRequest>(createRequest));
    }
}
