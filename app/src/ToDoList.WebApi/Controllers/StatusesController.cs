using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Entities;
using ToDoList.Core.Queries;
using ToDoList.Core.Response;

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
        public async Task<IEnumerable<StatusResponse>> Get()
        {
            IEnumerable<Status> statuses = await Mediator.Send(new GetAllQuery<Status>());
            return Mapper.Map<IEnumerable<StatusResponse>>(statuses);
        }

        [HttpGet("{id}")]
        public async Task<StatusResponse> Get(int id)
        {
            Status status = await Mediator.Send(new GetByIdQuery<Status>(id));
            return Mapper.Map<StatusResponse>(status);
        }
    }
}
