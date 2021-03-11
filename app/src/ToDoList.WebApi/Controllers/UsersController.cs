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
            IEnumerable<User> users = await Mediator.Send(new GetAllQuery<User>());
            return Mapper.Map<IEnumerable<UserResponse>>(users);
        }

        [HttpGet("{id}")]
        public async Task<UserResponse> Get(int id)
        {
            User user = await Mediator.Send(new GetByIdQuery<User>(id));
            return Mapper.Map<UserResponse>(user);
        }

        [HttpPost]
        public async Task Add([FromBody] UserCreateRequest createRequest)
        {
            User user = Mapper.Map<User>(createRequest);
            await Mediator.Send(new AddCommand<User>(user));
        }
    }
}
