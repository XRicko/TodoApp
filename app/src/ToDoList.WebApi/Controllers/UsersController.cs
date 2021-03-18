using System.Collections.Generic;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Response;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Base
    {
        public UsersController(IMediator mediator) : base(mediator)
        {

        }

        [HttpGet]
        public async Task<IEnumerable<UserResponse>> Get() =>
            await Mediator.Send(new GetAllQuery<User, UserResponse>());

        [HttpGet("{id}")]
        public async Task<UserResponse> Get(int id) =>
            await Mediator.Send(new GetByIdQuery<User, UserResponse>(id));

        [HttpPost]
        public async Task Add([FromBody] UserCreateRequest createRequest) =>
            await Mediator.Send(new AddCommand<UserCreateRequest>(createRequest));
    }
}
