using System;
using System.Security.Claims;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Commands.RefreshTokens;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Queries.RefreshTokens;
using ToDoList.Core.Mediator.Queries.Users;
using ToDoList.Core.Mediator.Requests;
using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Services;

namespace ToDoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Base
    {
        private readonly IAuthenticator authenticator;
        private readonly ITokenValidator tokenValidator;

        public AuthenticationController(IMediator mediator, IAuthenticator authenticationService, ITokenValidator validator) : base(mediator)
        {
            authenticator = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            tokenValidator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LoginAsync(UserRequest userRequest)
        {
            _ = userRequest ?? throw new ArgumentNullException(nameof(userRequest));

            var user = await Mediator.Send(new GetUserByNameAndPasswordQuery(userRequest.Name, userRequest.Password));

            if (user is null)
                return Unauthorized("Username or password is incorrect");

            return Ok(await authenticator.AuthenticateAsync(user));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RegisterAsync(UserRequest userRequest)
        {
            _ = userRequest ?? throw new ArgumentNullException(nameof(userRequest));

            var user = await Mediator.Send(new GetUserByNameAndPasswordQuery(userRequest.Name, userRequest.Password));

            if (user is not null)
                return Unauthorized("User exists");

            await Mediator.Send(new AddCommand<UserRequest>(userRequest));
            var createdUser = await Mediator.Send(new GetUserByNameAndPasswordQuery(userRequest.Name, userRequest.Password));

            return Ok(await authenticator.AuthenticateAsync(createdUser));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RefreshAsync([FromBody] string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentException($"'{nameof(refreshToken)}' cannot be null or whitespace", nameof(refreshToken));

            bool isValidRefreshToken = tokenValidator.ValidateRefreshKey(refreshToken);
            if (!isValidRefreshToken)
                return BadRequest("Invalid refresh token");

            var tokenResponse = await Mediator.Send(new GetRefreshTokenByTokenQuery(refreshToken));
            if (tokenResponse is null)
                return NotFound("Invalid refresh token");

            await Mediator.Send(new RemoveCommand<RefreshToken>(tokenResponse.Id));

            var user = await Mediator.Send(new GetByIdQuery<User, UserResponse>(tokenResponse.UserId));
            if (user is null)
                return NotFound("User not found");

            return Ok(await authenticator.AuthenticateAsync(user));
        }

        [Authorize]
        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> Logout()
        {
            string id = User?.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (string.IsNullOrWhiteSpace(id))
                return Unauthorized();

            await Mediator.Send(new RemoveAllRefreshTokensFromUserCommand(Convert.ToInt32(id)));

            return Ok();
        }
    }
}
