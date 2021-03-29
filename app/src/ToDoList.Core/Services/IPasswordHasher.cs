namespace ToDoList.Core.Services
{
    public interface IPasswordHasher
    {
        string Hash(string password, int iterations);
        bool Verify(string password, string hashedPassword);
    }
}
