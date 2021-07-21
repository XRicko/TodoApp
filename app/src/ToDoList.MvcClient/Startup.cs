using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using ToDoList.Extensions;
using ToDoList.MvcClient.API;
using ToDoList.MvcClient.Services;
using ToDoList.SharedClientLibrary.Services;

namespace ToDoList.MvcClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var apiConfig = Configuration.GetSection("WebApiConfig").GetValid<WebApiConfig>();

            services.AddSingleton(apiConfig);

            services.AddScoped<IApiInvoker, ApiInvoker>();

            var httpHandler = new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) };
            services.AddScoped(sp =>
            {
                var client = new HttpClient(httpHandler, false)
                {
                    BaseAddress = new Uri(apiConfig.BaseUrl)
                };

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return client;
            });

            //services.AddHttpClient("api", client =>
            //{
            //    client.BaseAddress = new Uri(apiConfig.BaseUrl);
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //});

            services.Configure<RequestLocalizationOptions>(options =>
            {
                string[] cultures = Configuration.GetSection("Cultures")
                                                 .GetChildren()
                                                 .Select(x => x.Key)
                                                 .ToArray();

                options.SetDefaultCulture("en");

                options.AddSupportedCultures(cultures);
                options.AddSupportedUICultures(cultures);
            });

            services.AddScoped<IViewModelService, ViewModelService>();

            services.AddScoped<ITokenStorage, CookieTokenStorage>();
            services.AddTransient<ITokenParser, JwtTokenParser>();

            services.AddLocalization();
            services.AddControllersWithViews()
                    .AddMvcLocalization();

            services.AddSession();
            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/Todo/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSession();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRequestLocalization();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
