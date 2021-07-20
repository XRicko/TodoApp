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
        private readonly HttpClient httpClient;
        private readonly ITokenStorage tokenStorage;

        public ApiInvoker(HttpClient client, ITokenStorage storage)
        {
            httpClient = client ?? throw new ArgumentNullException(nameof(client));
            tokenStorage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>(string route) where T : BaseModel
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));

            await AddAuthorizationHeaderAsync();

            return await GetContent(GetItems);

            Task<IEnumerable<T>> GetItems() => httpClient.GetFromJsonAsync<IEnumerable<T>>(route);
        }

        public async Task<T> GetItemAsync<T>(string routeWithParameters) where T : BaseModel
        {
            if (string.IsNullOrWhiteSpace(routeWithParameters))
                throw new ArgumentException($"'{nameof(routeWithParameters)}' cannot be null or whitespace", nameof(routeWithParameters));

            await AddAuthorizationHeaderAsync();

            return await GetContent(GetItem);

            Task<T> GetItem() => httpClient.GetFromJsonAsync<T>(routeWithParameters);
        }

        public async Task PostItemAsync<T>(string route, T item) where T : BaseModel
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));
            _ = item ?? throw new ArgumentNullException(nameof(item));

            await AddAuthorizationHeaderAsync();
            using var __ = await MakeRequest(Post);

            async Task<HttpResponseMessage> Post() => await httpClient.PostAsJsonAsync(route, item);
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

            await AddAuthorizationHeaderAsync();
            using var response = await MakeRequest(PostFile);

            return await response.Content.ReadFromJsonAsync<string>();

            async Task<HttpResponseMessage> PostFile() => await httpClient.PostAsync(route, formContent);
        }

        public async Task PutItemAsync<T>(string route, T item) where T : BaseModel
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));
            _ = item ?? throw new ArgumentNullException(nameof(item));

            await AddAuthorizationHeaderAsync();
            using var __ = await MakeRequest(Put);

            async Task<HttpResponseMessage> Put() => await httpClient.PutAsJsonAsync(route, item);
        }


        public async Task DeleteItemAsync(string route, int id)
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));

            await AddAuthorizationHeaderAsync();
            using var __ = await MakeRequest(Delete);

            async Task<HttpResponseMessage> Delete() => await httpClient.DeleteAsync(route + id);
        }

        public async Task LogOutAsync()
        {
            await AddAuthorizationHeaderAsync();
            using var __ = await MakeRequest(Logout);

            await RemoveTokensFromStorageAsync();
            httpClient.DefaultRequestHeaders.Authorization = null;

            Task<HttpResponseMessage> Logout() => httpClient.DeleteAsync("Authentication/Logout");
        }

        public async Task<AuthenticatedModel> AuthenticateUserAsync(string route, UserModel userModel)
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));
            _ = userModel ?? throw new ArgumentNullException(nameof(userModel));

            using var response = await httpClient.PostAsJsonAsync(route, userModel);
            response.EnsureSuccessStatusCode();

            var authenticatedModel = await response.Content.ReadFromJsonAsync<AuthenticatedModel>();

            await SetTokensInStorage(authenticatedModel);
            await AddAuthorizationHeaderAsync(authenticatedModel.AccessToken);

            return authenticatedModel;
        }

        public async Task AddAuthorizationHeaderAsync(string token = null)
        {
            string accessToken = token ?? await tokenStorage.GetTokenAsync("accessToken");

            if (AuthorizationHeaderMissing() && !string.IsNullOrWhiteSpace(accessToken))
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

            bool AuthorizationHeaderMissing() => httpClient.DefaultRequestHeaders.Authorization?.Parameter is null;
        }

        public async Task RefreshTokenAsync()
        {
            string refreshToken = await tokenStorage.GetTokenAsync("refreshToken");

            using var response = await httpClient.PostAsJsonAsync("Authentication/Refresh", refreshToken);
            response.EnsureSuccessStatusCode();

            var authenticatedModel = await response.Content.ReadFromJsonAsync<AuthenticatedModel>();

            await SetTokensInStorage(authenticatedModel);
            await AddAuthorizationHeaderAsync(authenticatedModel.AccessToken);
        }

        private async Task<T> GetContent<T>(Func<Task<T>> request)
        {
            try
            {
                return await request();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshTokenAsync();
                return await request();
            }
        }

        private async Task<HttpResponseMessage> MakeRequest(Func<Task<HttpResponseMessage>> request)
        {
            try
            {
                var response = await request();
                response.EnsureSuccessStatusCode();

                return response;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshTokenAsync();

                var response = await request();
                response.EnsureSuccessStatusCode();

                return response;
            }
        }

        private async Task SetTokensInStorage(AuthenticatedModel authenticatedModel)
        {
            _ = authenticatedModel ?? throw new ArgumentNullException(nameof(authenticatedModel));

            await tokenStorage.SetTokenAsync("accessToken", authenticatedModel.AccessToken);
            await tokenStorage.SetTokenAsync("refreshToken", authenticatedModel.RefreshToken);
        }

        private async Task RemoveTokensFromStorageAsync()
        {
            await tokenStorage.RemoveTokenAsync("accessToken");
            await tokenStorage.RemoveTokenAsync("refreshToken");
        }
    }
}