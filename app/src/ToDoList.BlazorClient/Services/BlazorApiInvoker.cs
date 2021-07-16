using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.Authorization;

using ToDoList.BlazorClient.Authentication;
using ToDoList.SharedClientLibrary.Models;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.BlazorClient.Services
{
    public class BlazorApiInvoker : ApiInvoker
    {
        private readonly AuthenticationStateProvider authStateProvider;

        public BlazorApiInvoker(HttpClient httpClient,
                                ITokenStorage tokenStorage,
                                AuthenticationStateProvider stateProvider)
            : base(httpClient, tokenStorage)
        {
            authStateProvider = stateProvider ?? throw new ArgumentNullException(nameof(stateProvider));
        }

        public override async Task<AuthenticatedModel> AuthenticateUserAsync(string route, UserModel userModel)
        {
            var authenticatedModel = await base.AuthenticateUserAsync(route, userModel);
            ((AuthStateProvider)authStateProvider).NotifyUserAuthentication(authenticatedModel.AccessToken);

            return authenticatedModel;
        }

        public override async Task LogoutAsync()
        {
            await base.LogoutAsync();
            ((AuthStateProvider)authStateProvider).NotifyUserLogout();
        }
    }
}
