using System.Collections.Generic;
using System.Threading.Tasks;

using ToDoList.SharedClientLibrary.Models;

namespace ToDoList.SharedClientLibrary.Services
{
    public interface IApiInvoker
    {
        Task<IEnumerable<T>> GetItemsAsync<T>(string route) where T : BaseModel;
        Task<T> GetItemAsync<T>(string routeWithParameters) where T : BaseModel;

        Task PostItemAsync<T>(string route, T item) where T : BaseModel;
        Task<string> PostFileAsync(string route, string fileName, byte[] fileBytes);

        Task PutItemAsync<T>(string route, T item) where T : BaseModel;
        Task DeleteItemAsync(string routeWithParameters);

        Task<AuthenticatedModel> AuthenticateUserAsync(string route, UserModel userModel);

        Task LogOutAsync();
        Task LogOutEverywhereAsync();

        Task AddAuthorizationHeaderAsync(string token = null);
        Task RefreshTokenAsync();
    }
}
