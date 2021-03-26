using System;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Requests.Update;
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

        [HttpGet("{id}")]
        public async Task<ImageResponse> Get(int id) =>
            await Mediator.Send(new GetByIdQuery<Image, ImageResponse>(id));

        [HttpGet]
        [Route("[action]/{name}")]
        public async Task<ImageResponse> GetByName(string name) =>
          await Mediator.Send(new GetByNameQuery<Image, ImageResponse>(name));

        [HttpPost]
        public async Task Add([FromBody] ImageCreateRequest createRequest)
        {
            _ = createRequest ?? throw new ArgumentNullException(nameof(createRequest));
            await Mediator.Send(new AddCommand<ImageCreateRequest>(createRequest));
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await Mediator.Send(new RemoveCommand<Image>(id));

        [HttpPut]
        public async Task Update([FromBody] ImageUpdateRequest updateRequest) 
        {
            _ = updateRequest ?? throw new ArgumentNullException(nameof(updateRequest));
            await Mediator.Send(new UpdateCommand<ImageUpdateRequest>(updateRequest));
        }
    }
}
