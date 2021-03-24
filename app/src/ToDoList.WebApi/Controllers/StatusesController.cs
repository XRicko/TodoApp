using System.Collections.Generic;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusesController : Base
    {
        public StatusesController(IMediator mediator) : base(mediator)
        {

        }

        [HttpGet]
        public async Task<IEnumerable<StatusResponse>> Get() =>
            await Mediator.Send(new GetAllQuery<Status, StatusResponse>());

        [HttpGet]
        [Route("[action]/{name}")]
        public async Task<StatusResponse> GetByName(string name) =>
            await Mediator.Send(new GetByNameQuery<Status, StatusResponse>(name));
    }
}
