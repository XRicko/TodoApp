using ToDoList.Core.Mediator.Response;

namespace ToDoList.WebApi.Jwt
{
    public interface ITokenGenerator
    {
        string GenerateAccessToken(UserResponse user);
        string GenerateRefreshToken();
    }
}
