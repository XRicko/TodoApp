namespace ToDoList.Core.Services
{
    public interface IPasswordHasher
    {
        string Hash(string password, int iterations);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
