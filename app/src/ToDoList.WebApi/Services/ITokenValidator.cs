namespace ToDoList.WebApi.Services
{
    public interface ITokenValidator
    {
        bool ValidateRefreshToken(string refreshToken);
    }
}