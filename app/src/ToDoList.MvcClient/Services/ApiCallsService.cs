using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using ToDoList.MvcClient.API;
using ToDoList.MvcClient.Models;

namespace ToDoList.MvcClient.Services
{
    public class ApiCallsService : IApiCallsService
    {
        public async Task<IEnumerable<T>> GetItemsAsync<T>(string route) where T : BaseModel
        {
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

        public async Task DeleteItemAsync(string route, int id)
        {
            using var response = await WebApiHelper.ApiClient.DeleteAsync(route + id);
            ValidateStatusCode(response);
        }

        private static void ValidateStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);
        }
    }
}
