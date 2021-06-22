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
                respone.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshTokenAsync();

                using var response = await Post();
                response.EnsureSuccessStatusCode();
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
                respone.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshTokenAsync();

                using var response = await Put();
                response.EnsureSuccessStatusCode();
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
                respone.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshTokenAsync();

                using var response = await Delete();
                response.EnsureSuccessStatusCode();
            }

            async Task<HttpResponseMessage> Delete() => await HttpClient.DeleteAsync(route + id);
        }

        public virtual async Task LogoutAsync()
        {
            await AddAuthorizationHeaderAsync();

            try
            {
                using var respone = await Logout();
                respone.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshTokenAsync();

                using var response = await Logout();
                response.EnsureSuccessStatusCode();
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
            response.EnsureSuccessStatusCode();

            var authenticatedModel = await response.Content.ReadFromJsonAsync<AuthenticatedModel>();

            await SetTokensInStorage(authenticatedModel);
            await AddAuthorizationHeaderAsync(authenticatedModel.AccessToken);

            return authenticatedModel;
        }

        private async Task RefreshTokenAsync()
        {
            string refreshToken = await TokenStorage.GetTokenAsync("refreshToken");

            using var response = await HttpClient.PostAsJsonAsync("Authentication/Refresh", refreshToken);
            response.EnsureSuccessStatusCode();

            var authenticatedModel = await response.Content.ReadFromJsonAsync<AuthenticatedModel>();

            await SetTokensInStorage(authenticatedModel);
            await AddAuthorizationHeaderAsync(authenticatedModel.AccessToken);
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

        public async Task<string> PostFileAsync(string route, string fileName, byte[] fileBytes)
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace.", nameof(route));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException($"'{nameof(fileName)}' cannot be null or whitespace.", nameof(fileName));

            _ = fileBytes ?? throw new ArgumentNullException(nameof(fileBytes));

            ByteArrayContent fileContent = new(fileBytes);

            using var formContent = new MultipartFormDataContent
            {
                { fileContent, "formFile", fileName }
            };

            try
            {
                using var response = await PostFile();
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<string>();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshTokenAsync();

                using var response = await PostFile();
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }

            async Task<HttpResponseMessage> PostFile() => await HttpClient.PostAsync(route, formContent);
        }
    }
}