using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using ToDoList.Core;
using ToDoList.Core.Entities;

namespace ToDoList.Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        // private readonly IMediator mediator;
        private readonly JwtTokenConfig jwtTokenConfig;

        private readonly List<User> users = new()
        {
            new User() { Name = "default", Password = "12346" },
            new User() { Name = "admin", Password = "qwerty" }
        };

        public TokenController(JwtTokenConfig tokenConfig)
        {
            jwtTokenConfig = tokenConfig;
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = users.SingleOrDefault(x => x.Name == username && x.Password == password);

            if (user is not null)
            {
                string token = GenerateToken(user);
                return Ok(new { AccessToken = token });
            }

            return Unauthorized();
        }

        private string GenerateToken(User user)
        {
            //var user = await Mediator.Send(new GetByNameQuery<User, UserResponse>(username));
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
