namespace ToDoList.WebApi.Services
{
    public interface ITokenValidator
    {
        bool ValidateRefreshKey(string refreshToken);
    }
}