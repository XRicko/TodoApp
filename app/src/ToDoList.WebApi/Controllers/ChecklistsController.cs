using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Queries.Checklists;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.Core.Mediator.Response;

namespace ToDoList.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ChecklistsController : Base
    {
        public ChecklistsController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route("[action]/{projectId}")]
        public async Task<IEnumerable<ChecklistResponse>> GetByProjectId(int projectId) =>
            await Mediator.Send(new GetChecklistsByProjectIdQuery(projectId));

        [HttpGet("{id}")]
        public async Task<ActionResult<ChecklistResponse>> Get(int id) =>
            await Mediator.Send(new GetByIdQuery<Checklist, ChecklistResponse>(id));

        [HttpGet("[action]")]
        public async Task<ActionResult<ChecklistResponse>> GetByProjectIdAndName(string name, int projectId)
        {
            return string.IsNullOrWhiteSpace(name)
                ? throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name))
                : await Mediator.Send(new GetChecklistByNameAndProjectIdQuery(name, projectId));
        }

        [HttpPost]
        public async Task<IActionResult> Add(ChecklistCreateRequest createRequest)
        {
            _ = createRequest ?? throw new ArgumentNullException(nameof(createRequest));

            await Mediator.Send(new AddCommand<ChecklistCreateRequest>(createRequest));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await Mediator.Send(new RemoveByIdCommand<Checklist>(id));
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> Update(ChecklistUpdateRequest updateRequest)
        {
            _ = updateRequest ?? throw new ArgumentNullException(nameof(updateRequest));

            await Mediator.Send(new UpdateCommand<ChecklistUpdateRequest>(updateRequest));

            return NoContent();
        }
    }
}
