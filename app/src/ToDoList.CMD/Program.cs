using Geocoding;
using Geocoding.Google;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ToDoList.Core.Controllers;
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers;
using ToDoList.Core.Queries;
using ToDoList.Infrastructure.Extensions;

namespace ToDoList
{
    class Program
    {
        private static string apiKey = null;

        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Getting things ready...");

            IGeocoder geocoder = new GoogleGeocoder() { ApiKey = apiKey };
            var addresses = await geocoder.ReverseGeocodeAsync(49.525675, 31.73423);
            Console.WriteLine(addresses.First().FormattedAddress);

            //var mediator = host.Services.GetRequiredService<IMediator>();

            //var user = await mediator.Send(new GetByIdQuery<User>(2));
            //Console.WriteLine(user.Name);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
         Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddInfrastructure(context.Configuration.GetConnectionString("DefaultConnection"));
                services.AddMediatR(Assembly.GetExecutingAssembly());

                apiKey = context.Configuration["GeocodingApi"];
            })
            .ConfigureLogging(config =>
            {
                config.ClearProviders();
                config.AddConsole();
            });
    }
}
