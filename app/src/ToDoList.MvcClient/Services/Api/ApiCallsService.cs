using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using ToDoList.MvcClient.API;
using ToDoList.MvcClient.Models;

namespace ToDoList.MvcClient.Services.Api
{
    [ExcludeFromCodeCoverage]
    public class ApiCallsService : IApiCallsService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly HttpContext httpContext;

        private readonly WebApiHelper webApiHelper;

        public ApiCallsService(IHttpContextAccessor httpAccessor, WebApiHelper apiHelper)
        {
            httpContextAccessor = httpAccessor ?? throw new ArgumentNullException(nameof(httpAccessor));
            httpContext = httpContextAccessor.HttpContext;

            webApiHelper = apiHelper;
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>(string route) where T : BaseModel
        {
            if (string.IsNullOrEmpty(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or empty", nameof(route));

            AddAuthenticationHeader();
            using var response = await webApiHelper.ApiClient.GetAsync(route);

            ValidateStatusCode(response);

            return await response.Content.ReadAsAsync<IEnumerable<T>>();
        }

        public async Task<T> GetItemAsync<T>(string routeWithParemeters) where T : BaseModel
        {
            if (string.IsNullOrEmpty(routeWithParemeters))
                throw new ArgumentException($"'{nameof(routeWithParemeters)}' cannot be null or empty", nameof(routeWithParemeters));

            using var response = await webApiHelper.ApiClient.GetAsync(routeWithParemeters);
            ValidateStatusCode(response);

            return await response.Content.ReadAsAsync<T>();
        }

        public async Task PostItemAsync<T>(string route, T item) where T : BaseModel
        {
            if (string.IsNullOrEmpty(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or empty", nameof(route));
            _ = item ?? throw new ArgumentNullException(nameof(item));

            using var response = await webApiHelper.ApiClient.PostAsJsonAsync(route, item);
            ValidateStatusCode(response);
        }

        public async Task PutItemAsync<T>(string route, T item) where T : BaseModel
        {
            if (string.IsNullOrEmpty(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or empty", nameof(route));
            _ = item ?? throw new ArgumentNullException(nameof(item));

            using var response = await webApiHelper.ApiClient.PutAsJsonAsync(route, item);
            ValidateStatusCode(response);
        }

        public async Task DeleteItemAsync(string route, int id)
        {
            if (string.IsNullOrEmpty(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or empty", nameof(route));

            using var response = await webApiHelper.ApiClient.DeleteAsync(route + id);
            ValidateStatusCode(response);
        }

        public async Task AuthenticateUserAsync(string route, UserModel userModel)
        {
            if (string.IsNullOrEmpty(route))
                throw new ArgumentException($"'{nameof(route)}' cannot be null or empty", nameof(route));
            _ = userModel ?? throw new ArgumentNullException(nameof(userModel));

            using var response = await webApiHelper.ApiClient.PostAsJsonAsync(route, userModel);

            ValidateStatusCode(response);

            string tokenJson = await response.Content.ReadAsStringAsync();
            string token = JsonSerializer.Deserialize<string>(tokenJson);

            httpContext.Response.Cookies.Append("Token", token, new CookieOptions { Expires = DateTime.Now.AddHours(3) });
        }

        private static void ValidateStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);
        }

        private void AddAuthenticationHeader()
        {
            webApiHelper.ApiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContext.Request.Cookies["Token"]);
        }
    }
}
