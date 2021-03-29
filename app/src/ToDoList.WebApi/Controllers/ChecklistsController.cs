using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Queries.Checklists;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.Core.Mediator.Response;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChecklistsController : Base
    {
        private string UserId => User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;

        public ChecklistsController(IMediator mediator) : base(mediator)
        {

        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IEnumerable<ChecklistResponse>> Get() =>
            await Mediator.Send(new GetChecklistsByUserIdQuery(Convert.ToInt32(UserId)));

        [HttpPost]
        public async Task Add([FromBody] ChecklistCreateRequest createRequest)
        {
            _ = createRequest ?? throw new ArgumentNullException(nameof(createRequest));
            await Mediator.Send(new AddCommand<ChecklistCreateRequest>(createRequest));
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id) =>
            await Mediator.Send(new RemoveCommand<Checklist>(id));

        [HttpPut]
        public async Task Update([FromBody] ChecklistUpdateRequest updateRequest)
        {
            _ = updateRequest ?? throw new ArgumentNullException(nameof(updateRequest));
            await Mediator.Send(new UpdateCommand<ChecklistUpdateRequest>(updateRequest));
        }
    }
}
