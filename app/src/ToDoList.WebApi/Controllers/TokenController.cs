using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using ToDoList.Core.Mediator.Queries.Users;
using ToDoList.Core.Mediator.Response;

namespace ToDoList.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TokenController : Base
    {
        private readonly JwtTokenConfig jwtTokenConfig;

        public TokenController(JwtTokenConfig tokenConfig, IMediator mediator) : base(mediator)
        {
            jwtTokenConfig = tokenConfig;
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(string username, string password)
        {
            var user = await Mediator.Send(new GetUserByNameAndPasswordQuery(username, password));

            if (user is not null)
            {
                string token = GenerateToken(user);
                return Ok(new { AccessToken = token });
            }

            return Unauthorized();
        }

        private string GenerateToken(UserResponse user)
        {
            var securityKey = jwtTokenConfig.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var token = new JwtSecurityToken(jwtTokenConfig.Issuer,
                                             jwtTokenConfig.Audience,
                                             claims,
                                             signingCredentials: credentials,
                                             expires: DateTime.Now.AddDays(1));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
