using System.Threading.Tasks;

using ToDoList.Core.Mediator.Response;
using ToDoList.WebApi.Jwt.Models;

namespace ToDoList.WebApi.Services
{
    public interface IAuthenticator
    {
        Task<AuthenticatedResponse> AuthenticateAsync(UserResponse user);
    }
}