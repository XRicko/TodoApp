namespace ToDoList.WebApi.Jwt.Models
{
    public record AuthenticatedResponse(string AccessToken, string RefreshToken);
}
