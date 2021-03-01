using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Commands;
using ToDoList.Core.DTOs;
using ToDoList.Core.Entities;
using ToDoList.Core.Queries;

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
        public async Task<ImageDTO> Get(int id)
        {
            Image image = await mediator.Send(new GetByIdQuery<Image>(id));
            return mapper.Map<ImageDTO>(image);
        }

        [HttpPost]
        public async Task Add([FromBody] ImageDTO imageDTO)
        {
            Image image = mapper.Map<Image>(imageDTO);
            await mediator.Send(new AddCommand<Image>(image));
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await mediator.Send(new RemoveCommand<Image>(id));

        [HttpPut]
        public async Task Update([FromBody] ImageDTO imageDTO)
        {
            Image checklist = mapper.Map<Image>(imageDTO);
            await mediator.Send(new UpdateCommand<Image>(checklist));
        }
    }
}
