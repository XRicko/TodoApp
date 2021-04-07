using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ToDoList.MvcClient.API
{
    [ExcludeFromCodeCoverage]
    public class WebApiHelper
    {
        private readonly WebApiConfig webApiConfig;

        public HttpClient ApiClient { get; set; }

        public WebApiHelper(WebApiConfig apiConfig)
        {
            webApiConfig = apiConfig;

            ApiClient = new HttpClient
            {
                BaseAddress = new Uri(webApiConfig.BaseUrl)
            };

            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
