using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Queries.Checklists;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.Core.Mediator.Response;
using ToDoList.Extensions;

namespace ToDoList.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChecklistsController : Base
    {
        private readonly IDistributedCache cache;

        private int UserId => Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        private string RecordKey => $"Checklists_User_{UserId}";

        public ChecklistsController(IMediator mediator, IDistributedCache distributedCache) : base(mediator)
        {
            cache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        [HttpGet]
        public async Task<IEnumerable<ChecklistResponse>> Get()
        {
            var checklists = await cache.GetRecordAsync<IEnumerable<ChecklistResponse>>(RecordKey);

            if (checklists is null)
            {
                checklists = await Mediator.Send(new GetChecklistsByUserIdQuery(UserId));
                await cache.SetRecordAsync(RecordKey, checklists);
            }

            return checklists;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChecklistResponse>> Get(int id) =>
            await Mediator.Send(new GetByIdQuery<Checklist, ChecklistResponse>(id));

        [HttpPost]
        public async Task<IActionResult> Add(ChecklistCreateRequest createRequest)
        {
            _ = createRequest ?? throw new ArgumentNullException(nameof(createRequest));

            await Mediator.Send(new AddCommand<ChecklistCreateRequest>(createRequest));
            await cache.RemoveAsync(RecordKey);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await Mediator.Send(new RemoveCommand<Checklist>(id));
            await cache.RemoveAsync(RecordKey);

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> Update(ChecklistUpdateRequest updateRequest)
        {
            _ = updateRequest ?? throw new ArgumentNullException(nameof(updateRequest));

            await Mediator.Send(new UpdateCommand<ChecklistUpdateRequest>(updateRequest));
            await cache.RemoveAsync(RecordKey);

            return NoContent();
        }
    }
}
