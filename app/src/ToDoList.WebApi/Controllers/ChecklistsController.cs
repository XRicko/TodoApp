using System.Collections.Generic;
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
    public class ChecklistsController : ControllerBase
    {
        private readonly IMediator mediator;

        public ChecklistsController(IMediator m)
        {
            mediator = m;
        }

        [HttpGet]
        public async Task<IEnumerable<Checklist>> Get() =>
           await mediator.Send(new GetAllQuery<Checklist>());

        [HttpGet("{id}")]
        public async Task<Checklist> Get(int id) =>
            await mediator.Send(new GetByIdQuery<Checklist>(id));

        [HttpPost]
        public async Task Add([FromBody] Checklist checklist) =>
            await mediator.Send(new AddCommand<Checklist>(checklist));

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await mediator.Send(new RemoveCommand<Checklist>(id));

        [HttpPut]
        public async Task Update([FromBody] Checklist checklist) =>
            await mediator.Send(new UpdateCommand<Checklist>(checklist));
    }
}
