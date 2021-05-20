using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using ToDoList.MvcClient.Models;

namespace ToDoList.MvcClient.Services.Api
{
    [ExcludeFromCodeCoverage]
    public class ApiInvoker : IApiInvoker
    {
        private readonly HttpClient httpClient;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ApiInvoker(HttpClient client, IHttpContextAccessor httpAccessor)
        {
            httpContextAccessor = httpAccessor ?? throw new ArgumentNullException(nameof(httpAccessor));
            httpClient = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>(string route) where T : BaseModel
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));

            AddAuthenticationHeader();

            using var response = await GetItems();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                using var newResponse = await RefreshTokenAndMakeNewRequest(GetItems);
                return await ReadItems(newResponse);
            }

            ValidateStatusCodeForSuccess(response);

            return await ReadItems(response);

            async Task<HttpResponseMessage> GetItems() => await httpClient.GetAsync(route);
            static async Task<IEnumerable<T>> ReadItems(HttpResponseMessage response) => await response.Content.ReadAsAsync<IEnumerable<T>>();
        }

        public async Task<T> GetItemAsync<T>(string routeWithParemeters) where T : BaseModel
        {
            if (string.IsNullOrWhiteSpace(routeWithParemeters))
                throw new ArgumentException($"'{nameof(routeWithParemeters)}' cannot be null or whitespace", nameof(routeWithParemeters));

            AddAuthenticationHeader();

            using var response = await GetItem();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                using var newResponse = await RefreshTokenAndMakeNewRequest(GetItem);
                return await ReadItem(newResponse);
            }

            ValidateStatusCodeForSuccess(response);

            return await ReadItem(response);

            async Task<HttpResponseMessage> GetItem() => await httpClient.GetAsync(routeWithParemeters);
            static async Task<T> ReadItem(HttpResponseMessage response) => await response.Content.ReadAsAsync<T>();
        }

        public async Task PostItemAsync<T>(string route, T item) where T : BaseModel
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));
            _ = item ?? throw new ArgumentNullException(nameof(item));

            AddAuthenticationHeader();

            using var response = await Post();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                using var newResponse = await RefreshTokenAndMakeNewRequest(Post);
                return;
            }

            ValidateStatusCodeForSuccess(response);

            async Task<HttpResponseMessage> Post() => await httpClient.PostAsJsonAsync(route, item);
        }

        public async Task PutItemAsync<T>(string route, T item) where T : BaseModel
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));
            _ = item ?? throw new ArgumentNullException(nameof(item));

            AddAuthenticationHeader();

            using var response = await Put();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                using var newResponse = await RefreshTokenAndMakeNewRequest(Put);
                return;
            }

            ValidateStatusCodeForSuccess(response);

            async Task<HttpResponseMessage> Put() => await httpClient.PutAsJsonAsync(route, item);
        }

        public async Task DeleteItemAsync(string route, int id)
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));

            AddAuthenticationHeader();

            using var response = await Delete();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                using var newResponse = await RefreshTokenAndMakeNewRequest(Delete);
                return;
            }

            ValidateStatusCodeForSuccess(response);

            async Task<HttpResponseMessage> Delete() => await httpClient.DeleteAsync(route + id);
        }

        public async Task AuthenticateUserAsync(string route, UserModel userModel)
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));
            _ = userModel ?? throw new ArgumentNullException(nameof(userModel));

            using var response = await httpClient.PostAsJsonAsync(route, userModel);
            ValidateStatusCodeForSuccess(response);

            var authenticatedModel = await response.Content.ReadAsAsync<AuthenticatedModel>();
            SetTokensInCookies(authenticatedModel.AccessToken, authenticatedModel.RefreshToken);
        }

        public async Task LogoutAsync()
        {
            AddAuthenticationHeader();

            using var respone = await Logout();

            if (respone.StatusCode == HttpStatusCode.Unauthorized)
            {
                using var newResponse = await RefreshTokenAndMakeNewRequest(Logout);
                return;
            }

            ValidateStatusCodeForSuccess(respone);

            async Task<HttpResponseMessage> Logout() => await httpClient.DeleteAsync("Authentication/Logout");
        }

        private static void ValidateStatusCodeForSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);
        }

        private void AddAuthenticationHeader(string token = null)
        {
            if (AuthorizationHeaderMissing())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    token ?? httpContextAccessor.HttpContext.Request.Cookies["Token"]);
            }

            bool AuthorizationHeaderMissing() => httpClient.DefaultRequestHeaders.Authorization?.Parameter is null;
        }

        private async Task<HttpResponseMessage> RefreshTokenAndMakeNewRequest(Func<Task<HttpResponseMessage>> request)
        {
            await RefreshToken();

            var response = await request();
            ValidateStatusCodeForSuccess(response);

            return response;
        }

        private async Task RefreshToken()
        {
            string refreshToken = httpContextAccessor.HttpContext.Request.Cookies["RefreshToken"];

            using var response = await httpClient.PostAsJsonAsync("Authentication/Refresh", refreshToken);
            var authenticatedModel = await response.Content.ReadAsAsync<AuthenticatedModel>();

            SetTokensInCookies(authenticatedModel.AccessToken, authenticatedModel.RefreshToken);
            AddAuthenticationHeader(authenticatedModel.AccessToken);
        }

        private void SetTokensInCookies(string accessToken, string refreshToken)
        {
            httpContextAccessor.HttpContext.Response.Cookies.Append("Token",
                                                                    accessToken,
                                                                    new CookieOptions { Expires = DateTime.Now.AddMinutes(1) });
            httpContextAccessor.HttpContext.Response.Cookies.Append("RefreshToken",
                                                                    refreshToken,
                                                                    new CookieOptions { Expires = DateTime.Now.AddMonths(3) });
        }
    }
}
