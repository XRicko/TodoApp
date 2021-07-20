using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.Authorization;

using ToDoList.BlazorClient.Authentication;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.BlazorClient.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IApiInvoker apiInvoker;
        private readonly AuthenticationStateProvider authenticationStateProvider;

        public AuthenticationService(IApiInvoker invoker, AuthenticationStateProvider authenticationState)
        {
            apiInvoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
            authenticationStateProvider = authenticationState ?? throw new ArgumentNullException(nameof(authenticationState));
        }

        public async Task AuthenticateAsync(string action, UserModel user)
        {
            var authenticatedModel = await apiInvoker.AuthenticateUserAsync($"Authentication/{action}", user);
            ((AuthStateProvider)authenticationStateProvider).NotifyUserAuthentication(authenticatedModel.AccessToken);
        }

        public async Task LogOutAsync()
        {
            await apiInvoker.LogOutAsync();
            ((AuthStateProvider)authenticationStateProvider).NotifyUserLogout();
        }
    }
}
