using System.Collections.Generic;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Queries;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.Core.Mediator.Response;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChecklistsController : Base
    {
        public ChecklistsController(IMediator mediator) : base(mediator)
        {

        }

        [HttpGet]
        public async Task<IEnumerable<ChecklistResponse>> Get() =>
            await Mediator.Send(new GetAllQuery<Checklist, ChecklistResponse>());

        [HttpGet("{id}")]
        public async Task<ChecklistResponse> Get(int id) =>
            await Mediator.Send(new GetByIdQuery<Checklist, ChecklistResponse>(id));

        [HttpPost]
        public async Task Add([FromBody] ChecklistCreateRequest createRequest) =>
            await Mediator.Send(new AddCommand<ChecklistCreateRequest>(createRequest));

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await Mediator.Send(new RemoveCommand<Checklist>(id));

        [HttpPut]
        public async Task Update([FromBody] ChecklistUpdateRequest updateRequest) =>
            await Mediator.Send(new UpdateCommand<ChecklistUpdateRequest>(updateRequest));
    }
}
