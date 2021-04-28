using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using ToDoList.Diagnostic;
using ToDoList.Extensions;
using ToDoList.MvcClient.API;
using ToDoList.MvcClient.Services;
using ToDoList.MvcClient.Services.Api;

namespace ToDoList.MvcClient
{
    [ExcludeFromCodeCoverage]
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
            services.AddHttpClient<IApiInvoker, ApiInvoker>(client =>
            {
                client.BaseAddress = new Uri(apiConfig.BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultures = new CultureInfo[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("uk")
                };

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });

            services.AddScoped<IViewModelService, ViewModelService>();
            services.AddScoped<IImageAddingService, ImageAddingService>();

            services.AddSingleton<ProccessTimeCounterSource>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddControllersWithViews(options => options.Filters.Add<ProccesTimeActionFilterAttribute>())
                    .AddMvcLocalization();

            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddSession();

            services.AddDistributedMemoryCache();

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

            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

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
