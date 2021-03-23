using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using ToDoList.MvcClient.API;
using ToDoList.MvcClient.Models;

namespace ToDoList.MvcClient.Services.Api
{
    public class ApiCallsService : IApiCallsService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly HttpContext httpContext;

        public ApiCallsService(IHttpContextAccessor httpAccessor)
        {
            httpContextAccessor = httpAccessor;
            httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>(string route) where T : BaseModel
        {
            AddAuthenticationHeader();
            using var response = await WebApiHelper.ApiClient.GetAsync(route);

            ValidateStatusCode(response);

            return await response.Content.ReadAsAsync<IEnumerable<T>>();
        }

        public async Task<T> GetItemAsync<T>(string routeWithParemeters) where T : BaseModel
        {
            using var response = await WebApiHelper.ApiClient.GetAsync(routeWithParemeters);
            ValidateStatusCode(response);

            return await response.Content.ReadAsAsync<T>();
        }

        public async Task PostItemAsync<T>(string route, T item) where T : BaseModel
        {
            using var response = await WebApiHelper.ApiClient.PostAsJsonAsync(route, item);
            ValidateStatusCode(response);
        }

        public async Task PutItemAsync<T>(string route, T item) where T : BaseModel
        {
            using var response = await WebApiHelper.ApiClient.PutAsJsonAsync(route, item);
            ValidateStatusCode(response);
        }

        public async Task DeleteItemAsync(string route, int id)
        {
            using var response = await WebApiHelper.ApiClient.DeleteAsync(route + id);
            ValidateStatusCode(response);
        }

        public async Task AuthenticateUserAsync(string route, UserModel userModel)
        {
            using var response = await WebApiHelper.ApiClient.PostAsJsonAsync(route, userModel);

            ValidateStatusCode(response);

            string tokenJson = await response.Content.ReadAsStringAsync();
            string token = JsonSerializer.Deserialize<string>(tokenJson);

            httpContext.Response.Cookies.Append("Token", token, new CookieOptions { Expires = DateTime.Now.AddHours(3)});
        }

        private static void ValidateStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);
        }

        private void AddAuthenticationHeader()
        {
            WebApiHelper.ApiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContext.Request.Cookies["Token"]);
        }
    }
}
