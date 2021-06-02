using System;
using System.Threading.Tasks;

using Blazored.LocalStorage;

using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.BlazorClient.Services
{
    public class LocalTokenStorage : ITokenStorage
    {
        private readonly ILocalStorageService localStorage;

        public LocalTokenStorage(ILocalStorageService storageService)
        {
            localStorage = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task<string> GetTokenAsync(string key) =>
            await localStorage.GetItemAsStringAsync(key);

        public async Task SetTokenAsync(string key, string token) =>
            await localStorage.SetItemAsStringAsync(key, token);

        public async Task RemoveTokenAsync(string key) =>
            await localStorage.RemoveItemAsync(key);
    }
}
