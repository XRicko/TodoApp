using System.Threading.Tasks;

namespace ToDoList.SharedClientLibrary.Services
{
    public interface ITokenStorage
    {
        Task<string> GetTokenAsync(string key);

        Task SetTokenAsync(string key, string token);
        Task RemoveTokenAsync(string key);
    }
}
