using System;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Queries.Users;
using ToDoList.Core.Mediator.Requests;
using ToDoList.WebApi.Jwt;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Base
    {
        private readonly ITokenGenerator tokenGenerator;

        public UserController(IMediator mediator, ITokenGenerator generator) : base(mediator)
        {
            tokenGenerator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LoginAsync(UserRequest userRequest)
        {
            _ = userRequest ?? throw new ArgumentNullException(nameof(userRequest));

            var user = await Mediator.Send(new GetUserByNameAndPasswordQuery(userRequest.Name, userRequest.Password));

            if (user is null)
                return Unauthorized("No user found");

            string token = tokenGenerator.GenerateToken(user.Id, user.Name);
            return Ok(token);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RegisterAsync(UserRequest userCreateRequest)
        {
            _ = userCreateRequest ?? throw new ArgumentNullException(nameof(userCreateRequest));

            var user = await Mediator.Send(new GetUserByNameAndPasswordQuery(userCreateRequest.Name, userCreateRequest.Password));

            if (user is not null)
                return BadRequest("User exists");

            await Mediator.Send(new AddCommand<UserRequest>(userCreateRequest));
            var createdUser = await Mediator.Send(new GetUserByNameAndPasswordQuery(userCreateRequest.Name, userCreateRequest.Password));

            string token = tokenGenerator.GenerateToken(createdUser.Id, createdUser.Name);
            return Ok(token);
        }
    }
}
