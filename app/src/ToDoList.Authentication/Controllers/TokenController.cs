using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using ToDoList.Core.Entities;

namespace ToDoList.Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController
    {
        private readonly IMediator mediator;
        private readonly JwtTokenConfig jwtTokenConfig;

        public TokenController(IMediator mediatr, JwtTokenConfig tokenConfig)
        {
            mediator = mediatr;
            jwtTokenConfig = tokenConfig;
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            return string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)
                ? BadRequest()
                : new JsonResult(GenerateToken(username));
        }

        private string GenerateToken(string username)
        {
            //var user = await Mediator.Send(new GetByNameQuery<User, UserResponse>(username));
            var securityKey = jwtTokenConfig.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            User user = new() { Name = "default", Password = "123456" };

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddDays(1).ToString())
            };

            var token = new JwtSecurityToken(claims: claims, signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
