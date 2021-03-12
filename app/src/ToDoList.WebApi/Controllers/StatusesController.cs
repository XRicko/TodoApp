using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Queries;
using ToDoList.Core.Mediator.Response;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusesController : Base
    {
        public StatusesController(IMediator mediator, IMapper mapper) : base(mediator, mapper)
        {

        }

        [HttpGet]
        public async Task<IEnumerable<StatusResponse>> Get() =>
            await Mediator.Send(new GetAllQuery<Status, StatusResponse>());

        [HttpGet("{id}")]
        public async Task<StatusResponse> Get(int id) =>
            await Mediator.Send(new GetByIdQuery<Status, StatusResponse>(id));
    }
}
