using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using ToDoList.MvcClient.Models;

namespace ToDoList.MvcClient.Services.Api
{
    [ExcludeFromCodeCoverage]
    public class ApiInvoker : IApiInvoker
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly HttpClient httpClient;

        public ApiInvoker(IHttpContextAccessor httpAccessor, HttpClient client)
        {
            httpContextAccessor = httpAccessor ?? throw new ArgumentNullException(nameof(httpAccessor));
            httpClient = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>(string route) where T : BaseModel
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));

            AddAuthenticationHeader();
            using var response = await httpClient.GetAsync(route);

            ValidateStatusCode(response);

            return await response.Content.ReadAsAsync<IEnumerable<T>>();
        }

        public async Task<T> GetItemAsync<T>(string routeWithParemeters) where T : BaseModel
        {
            if (string.IsNullOrWhiteSpace(routeWithParemeters))
                throw new ArgumentException($"'{nameof(routeWithParemeters)}' cannot be null or whitespace", nameof(routeWithParemeters));

            using var response = await httpClient.GetAsync(routeWithParemeters);
            ValidateStatusCode(response);

            return await response.Content.ReadAsAsync<T>();
        }

        public async Task PostItemAsync<T>(string route, T item) where T : BaseModel
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));
            _ = item ?? throw new ArgumentNullException(nameof(item));

            using var response = await httpClient.PostAsJsonAsync(route, item);
            ValidateStatusCode(response);
        }

        public async Task PutItemAsync<T>(string route, T item) where T : BaseModel
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));
            _ = item ?? throw new ArgumentNullException(nameof(item));

            using var response = await httpClient.PutAsJsonAsync(route, item);
            ValidateStatusCode(response);
        }

        public async Task DeleteItemAsync(string route, int id)
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));

            using var response = await httpClient.DeleteAsync(route + id);
            ValidateStatusCode(response);
        }

        public async Task AuthenticateUserAsync(string route, UserModel userModel)
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace", nameof(route));
            _ = userModel ?? throw new ArgumentNullException(nameof(userModel));

            using var response = await httpClient.PostAsJsonAsync(route, userModel);

            ValidateStatusCode(response);

            string tokenJson = await response.Content.ReadAsStringAsync();
            string token = JsonSerializer.Deserialize<string>(tokenJson);

            httpContextAccessor.HttpContext.Response.Cookies.Append("Token", token, new CookieOptions { Expires = DateTime.Now.AddHours(3) });
        }

        private static void ValidateStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);
        }

        private void AddAuthenticationHeader()
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.HttpContext.Request.Cookies["Token"]);
        }
    }
}
