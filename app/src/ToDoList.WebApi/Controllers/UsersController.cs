using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Commands;
using ToDoList.Core.Entities;
using ToDoList.Core.Queries;
using ToDoList.Core.Response;
using ToDoList.WebApi.Requests.Create;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Base
    {
        public UsersController(IMediator mediator, IMapper mapper) : base(mediator, mapper)
        {

        }

        [HttpGet]
        public async Task<IEnumerable<UserResponse>> Get()
        {
            IEnumerable<User> users = await mediator.Send(new GetAllQuery<User>());
            return mapper.Map<IEnumerable<UserResponse>>(users);
        }

        [HttpGet("{id}")]
        public async Task<UserResponse> Get(int id)
        {
            User user = await mediator.Send(new GetByIdQuery<User>(id));
            return mapper.Map<UserResponse>(user);
        }

        [HttpPost]
        public async Task Add([FromBody] UserCreateRequest createRequest)
        {
            User user = mapper.Map<User>(createRequest);
            await mediator.Send(new AddCommand<User>(user));
        }
    }
}
