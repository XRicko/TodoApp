namespace ToDoList.WebApi.Jwt
{
    public interface ITokenGenerator
    {
        string GenerateToken(int id, string username);
    }
}
