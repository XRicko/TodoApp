using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ToDoList.MvcClient.API
{
    internal static class WebApiHelper
    {
        public static HttpClient WebApiClient { get; set; }

        public static void InitializeClient()
        {
            WebApiClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44391/api/")
            };

            WebApiClient.DefaultRequestHeaders.Accept.Clear();
            WebApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
