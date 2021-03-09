using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ToDoList.MvcClient.API
{
    internal static class ApiHelper
    {
        public static HttpClient ApiClient { get; set; }

        public static void InitializeClient()
        {
            ApiClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44391/api/")
            };

            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
