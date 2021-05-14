using ToDoList.Core.Mediator.Response;

namespace ToDoList.WebApi.Services
{
    public interface ITokenGenerator
    {
        string GenerateAccessToken(UserResponse user);
        string GenerateRefreshToken();
    }
}
