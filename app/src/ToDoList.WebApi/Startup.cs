using Geocoding;
using Geocoding.Google;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using ToDoList.Core.Mediator.Queries;
using ToDoList.Core.Mediator.Response;
using ToDoList.Core.Services;
using ToDoList.Infrastructure.Extensions;

namespace ToDoList.WebApi
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
            services.AddInfrastructure(Configuration.GetConnectionString("DefaultConnection"));

            services.Configure<ApiOptions>(Configuration.GetSection(ApiOptions.Apis));

            services.AddScoped<IGeocoder>(x => new GoogleGeocoder(x.GetService<IOptions<ApiOptions>>().Value.GoogleApiKey));

            services.AddTransient<IGeocodingService, GeocodingService>();
            services.AddTransient<ICreateTodoItemResponseWithAddressService, CreateTodoItemResponseWithAddressService>();

            services.AddAutoMapper(typeof(CategoryResponse));
            services.AddMediatR(typeof(GetAllQuery<,>));

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDoList.WebApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDoList.WebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
