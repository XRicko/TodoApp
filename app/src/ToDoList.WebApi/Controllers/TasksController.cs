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
    public class TasksController : ControllerBase
    {
        private readonly IMediator mediator;

        public TasksController(IMediator m)
        {
            mediator = m;
        }

        [HttpGet]
        public async Task<IEnumerable<ChecklistItem>> Get() =>
            await mediator.Send(new GetAllQuery<ChecklistItem>());

        [HttpGet("{id}")]
        public async Task<ChecklistItem> Get(int id) =>
            await mediator.Send(new GetByIdQuery<ChecklistItem>(id));

        [HttpPost]
        public async Task Add([FromBody] ChecklistItem checklistItem) =>
            await mediator.Send(new AddCommand<ChecklistItem>(checklistItem));

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await mediator.Send(new RemoveCommand<ChecklistItem>(id));

        [HttpPut]
        public async Task Update([FromBody] ChecklistItem checklistItem) =>
            await mediator.Send(new UpdateCommand<ChecklistItem>(checklistItem));
    }
}
