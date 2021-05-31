using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.Authorization;

using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.BlazorClient.Authentication
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient httpClient;
        private readonly ITokenParser tokenParser;
        private readonly ITokenStorage tokenStorage;

        private readonly AuthenticationState anonymous;

        public AuthStateProvider(HttpClient client, ITokenParser parser, ITokenStorage storage)
        {
            httpClient = client ?? throw new ArgumentNullException(nameof(client));
            tokenParser = parser ?? throw new ArgumentNullException(nameof(parser));
            tokenStorage = storage ?? throw new ArgumentNullException(nameof(storage));

            anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string token = await tokenStorage.GetTokenAsync("accessToken");

            if (string.IsNullOrWhiteSpace(token))
                return anonymous;

            var claimsPrincipal = tokenParser.GetClaimsPrincipal(token);
            var expiryDate = tokenParser.GetExpiryDate(token, claimsPrincipal);

            if (DateTimeOffset.Now > expiryDate)
                return anonymous;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

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
