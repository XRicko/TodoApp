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
    public class ImagesController : Base
    {
        public ImagesController(IMediator mediator, IMapper mapper) : base(mediator, mapper)
        {

        }

        [HttpGet("{id}")]
        public async Task<ImageResponse> Get(int id)
        {
            Image image = await Mediator.Send(new GetByIdQuery<Image>(id));
            return Mapper.Map<ImageResponse>(image);
        }

        [HttpPost]
        public async Task Add([FromBody] ImageCreateRequest createRequest)
        {
            Image image = Mapper.Map<Image>(createRequest);
            await Mediator.Send(new AddCommand<Image>(image));
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await Mediator.Send(new RemoveCommand<Image>(id));

        [HttpPut]
        public async Task Update([FromBody] ImageUpdateRequest updateRequest)
        {
            Image checklist = Mapper.Map<Image>(updateRequest);
            await Mediator.Send(new UpdateCommand<Image>(checklist));
        }
    }
}
