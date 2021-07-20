using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Blazored.LocalStorage;
using Blazored.Modal;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using ToDoList.BlazorClient.Authentication;
using ToDoList.BlazorClient.Extensions;
using ToDoList.BlazorClient.Services;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.BlazorClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped<ITokenStorage, LocalTokenStorage>();
            builder.Services.AddScoped<ITokenParser, JwtTokenParser>();

            builder.Services.AddScoped<IApiInvoker, ApiInvoker>();
            builder.Services.AddSingleton<Notifier>();

            builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddBlazoredModal();

            builder.Services.AddAuthorizationCore();

            builder.Services.AddLocalization();

            builder.Services.AddScoped(sp =>
            {
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(builder.Configuration["baseApiUrl"])
                };

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return httpClient;
            });

            var webAssemblyHost = builder.Build();

            await webAssemblyHost.SetDefaultCultureAsync();
            await webAssemblyHost.RunAsync();
        }
    }
}
