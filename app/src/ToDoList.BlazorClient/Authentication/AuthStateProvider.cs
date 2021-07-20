using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.Authorization;

using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.BlazorClient.Authentication
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly IApiInvoker apiInvoker;

        private readonly ITokenParser tokenParser;
        private readonly ITokenStorage tokenStorage;

        private readonly AuthenticationState anonymous;

        public AuthStateProvider(IApiInvoker invoker, ITokenParser parser, ITokenStorage storage)
        {
            apiInvoker = invoker ?? throw new ArgumentNullException(nameof(invoker));

            tokenParser = parser ?? throw new ArgumentNullException(nameof(parser));
            tokenStorage = storage ?? throw new ArgumentNullException(nameof(storage));

            anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string token = await tokenStorage.GetTokenAsync("accessToken");
            string refreshToken = await tokenStorage.GetTokenAsync("refreshToken");

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(refreshToken))
                return anonymous;

            var claimsPrincipal = tokenParser.GetClaimsPrincipal(token);
            var expiryDate = tokenParser.GetExpiryDate(token, claimsPrincipal);

            if (DateTimeOffset.Now > expiryDate)
            {
                await apiInvoker.RefreshTokenAsync();

                token = await tokenStorage.GetTokenAsync("accessToken");
                claimsPrincipal = tokenParser.GetClaimsPrincipal(token);
            }

            await apiInvoker.AddAuthorizationHeaderAsync(token);

            return new AuthenticationState(claimsPrincipal);
        }

        public void NotifyUserAuthentication(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException($"'{nameof(token)}' cannot be null or whitespace.", nameof(token));

            var authenticatedUser = tokenParser.GetClaimsPrincipal(token);

            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(anonymous);
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
