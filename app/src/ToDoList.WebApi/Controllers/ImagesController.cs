using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Commands;
using ToDoList.Core.Entities;
using ToDoList.Core.Queries;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IMediator mediator;

        public ImagesController(IMediator m)
        {
            mediator = m;
        }

        [HttpGet("{id}")]
        public async Task<Image> Get(int id) =>
            await mediator.Send(new GetByIdQuery<Image>(id));

        [HttpPost]
        public async Task Add([FromBody] Image image) =>
            await mediator.Send(new AddCommand<Image>(image));

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await mediator.Send(new RemoveCommand<Image>(id));
    }
}
