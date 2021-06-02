using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

using ToDoList.SharedClientLibrary.Models;

namespace ToDoList.SharedClientLibrary.Services
{
    public class ApiInvoker : IApiInvoker
    {
        protected HttpClient HttpClient { get; }
        protected ITokenStorage TokenStorage { get; }

        public ApiInvoker(HttpClient httpClient, ITokenStorage tokenStorage)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            TokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>(string route) where T : BaseModel
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));

            await AddAuthorizationHeaderAsync();

            try
            {
                return await GetItems();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshTokenAsync();
                return await GetItems();
            }

            Task<IEnumerable<T>> GetItems() => HttpClient.GetFromJsonAsync<IEnumerable<T>>(route);
        }

        public async Task<T> GetItemAsync<T>(string routeWithParameters) where T : BaseModel
        {
            if (string.IsNullOrWhiteSpace(routeWithParameters))
                throw new ArgumentException($"'{nameof(routeWithParameters)}' cannot be null or whitespace", nameof(routeWithParameters));

            await AddAuthorizationHeaderAsync();

            try
            {
                return await GetItem();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshTokenAsync();
                return await GetItem();
            }

            Task<T> GetItem() => HttpClient.GetFromJsonAsync<T>(routeWithParameters);
        }

        public async Task PostItemAsync<T>(string route, T item) where T : BaseModel
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));
            _ = item ?? throw new ArgumentNullException(nameof(item));

            await AddAuthorizationHeaderAsync();

            try
            {
                using var respone = await Post();
                ValidateStatusCodeForSuccess(respone);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshTokenAsync();

                using var response = await Post();
                ValidateStatusCodeForSuccess(response);
            }

            async Task<HttpResponseMessage> Post() => await HttpClient.PostAsJsonAsync(route, item);
        }

        public async Task PutItemAsync<T>(string route, T item) where T : BaseModel
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));
            _ = item ?? throw new ArgumentNullException(nameof(item));

            await AddAuthorizationHeaderAsync();

            try
            {
                using var respone = await Put();
                ValidateStatusCodeForSuccess(respone);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshTokenAsync();

                using var response = await Put();
                ValidateStatusCodeForSuccess(response);
            }

            async Task<HttpResponseMessage> Put() => await HttpClient.PutAsJsonAsync(route, item);
        }


        public async Task DeleteItemAsync(string route, int id)
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));

            await AddAuthorizationHeaderAsync();

            try
            {
                using var respone = await Delete();
                ValidateStatusCodeForSuccess(respone);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshTokenAsync();

                using var response = await Delete();
                ValidateStatusCodeForSuccess(response);
            }

            async Task<HttpResponseMessage> Delete() => await HttpClient.DeleteAsync(route + id);
        }

        public virtual async Task LogoutAsync()
        {
            await AddAuthorizationHeaderAsync();

            try
            {
                using var respone = await Logout();
                ValidateStatusCodeForSuccess(respone);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshTokenAsync();

                using var response = await Logout();
                ValidateStatusCodeForSuccess(response);
            }

            await RemoveTokensFromStorageAsync();
            HttpClient.DefaultRequestHeaders.Authorization = null;

            Task<HttpResponseMessage> Logout() => HttpClient.DeleteAsync("Authentication/Logout");
        }

        public virtual async Task<AuthenticatedModel> AuthenticateUserAsync(string route, UserModel userModel)
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));
            _ = userModel ?? throw new ArgumentNullException(nameof(userModel));

            using var response = await HttpClient.PostAsJsonAsync(route, userModel);
            ValidateStatusCodeForSuccess(response);

            var authenticatedModel = await response.Content.ReadFromJsonAsync<AuthenticatedModel>();

            await SetTokensInStorage(authenticatedModel);
            await AddAuthorizationHeaderAsync(authenticatedModel.AccessToken);

            return authenticatedModel;
        }

        private async Task RefreshTokenAsync()
        {
            string refreshToken = await TokenStorage.GetTokenAsync("refreshToken");

            using var response = await HttpClient.PostAsJsonAsync("Authentication/Refresh", refreshToken);
            ValidateStatusCodeForSuccess(response);

            var authenticatedModel = await response.Content.ReadFromJsonAsync<AuthenticatedModel>();

            await SetTokensInStorage(authenticatedModel);
            await AddAuthorizationHeaderAsync(authenticatedModel.AccessToken);
        }

        private static void ValidateStatusCodeForSuccess(HttpResponseMessage response)
        {
            _ = response ?? throw new ArgumentNullException(nameof(response));

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Response status code does not indicate success: " +
                    $"{(int)response.StatusCode} ({response.ReasonPhrase}).", null, response.StatusCode);
            }
        }

        private async Task SetTokensInStorage(AuthenticatedModel authenticatedModel)
        {
            _ = authenticatedModel ?? throw new ArgumentNullException(nameof(authenticatedModel));

            await TokenStorage.SetTokenAsync("accessToken", authenticatedModel.AccessToken);
            await TokenStorage.SetTokenAsync("refreshToken", authenticatedModel.RefreshToken);
        }

        private async Task RemoveTokensFromStorageAsync()
        {
            await TokenStorage.RemoveTokenAsync("accessToken");
            await TokenStorage.RemoveTokenAsync("refreshToken");
        }

        private async Task AddAuthorizationHeaderAsync(string token = null)
        {
            string accessToken = token ?? await TokenStorage.GetTokenAsync("accessToken");

            if (AuthorizationHeaderMissing() && !string.IsNullOrWhiteSpace(accessToken))
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

            bool AuthorizationHeaderMissing() => HttpClient.DefaultRequestHeaders.Authorization?.Parameter is null;
        }
    }
}