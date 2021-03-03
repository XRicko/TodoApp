using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Commands;
using ToDoList.Core.Entities;
using ToDoList.Core.Queries;
using ToDoList.Core.Response;
using ToDoList.WebApi.Requests.Create;
using ToDoList.WebApi.Requests.Update;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public ImagesController(IMediator m, IMapper map)
        {
            mediator = m;
            mapper = map;
        }

        [HttpGet("{id}")]
        public async Task<ImageResponse> Get(int id)
        {
            Image image = await mediator.Send(new GetByIdQuery<Image>(id));
            return mapper.Map<ImageResponse>(image);
        }

        [HttpPost]
        public async Task Add([FromBody] ImageCreateRequest createRequest)
        {
            Image image = mapper.Map<Image>(createRequest);
            await mediator.Send(new AddCommand<Image>(image));
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await mediator.Send(new RemoveCommand<Image>(id));

        [HttpPut]
        public async Task Update([FromBody] ImageUpdateRequest updateRequest)
        {
            Image checklist = mapper.Map<Image>(updateRequest);
            await mediator.Send(new UpdateCommand<Image>(checklist));
        }
    }
}
