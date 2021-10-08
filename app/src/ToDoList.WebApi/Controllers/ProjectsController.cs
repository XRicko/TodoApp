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
using ToDoList.Core.Mediator.Queries.Projects;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.Core.Mediator.Response;
using ToDoList.Extensions;

namespace ToDoList.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProjectsController : Base
    {
        private readonly IDistributedCache cache;

        private int UserId => Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        private string RecordKey => $"Projects_User_{UserId}";

        public ProjectsController(IMediator mediator, IDistributedCache distributedCache) : base(mediator)
        {
            cache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        [HttpGet]
        public async Task<IEnumerable<ProjectResponse>> Get()
        {
            var projects = await cache.GetRecordAsync<IEnumerable<ProjectResponse>>(RecordKey);

            if (projects is null)
            {
                projects = await Mediator.Send(new GetProjectsByUserIdQuery(UserId));
                await cache.SetRecordAsync(RecordKey, projects);
            }

            return projects;
        }

        [HttpGet]
        [Route("[action]/{name}")]
        public async Task<ActionResult<ProjectResponse>> GetByName(string name)
        {
            return string.IsNullOrWhiteSpace(name)
                ? throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name))
                : await Mediator.Send(new GetProjectByNameAndUserIdQuery(name, UserId));
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProjectCreateRequest createRequest)
        {
            _ = createRequest ?? throw new ArgumentNullException(nameof(createRequest));

            await Mediator.Send(new AddCommand<ProjectCreateRequest>(createRequest));
            await cache.RemoveAsync(RecordKey);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await Mediator.Send(new RemoveByIdCommand<Project>(id));
            await cache.RemoveAsync(RecordKey);

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> Update(ProjectUpdateRequest updateRequest)
        {
            _ = updateRequest ?? throw new ArgumentNullException(nameof(updateRequest));

            await Mediator.Send(new UpdateCommand<ProjectUpdateRequest>(updateRequest));
            await cache.RemoveAsync(RecordKey);

            return NoContent();
        }
    }
}
