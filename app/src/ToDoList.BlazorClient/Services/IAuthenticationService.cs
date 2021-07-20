using System.Threading.Tasks;

using ToDoList.SharedClientLibrary.Models;

namespace ToDoList.BlazorClient.Services
{
    public interface IAuthenticationService
    {
        Task AuthenticateAsync(string action, UserModel user);
        Task LogOutAsync();
    }
}