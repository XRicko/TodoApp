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
        static async Task Main(string[] args)
        {
            //IGeocoder geocoder = new GoogleGeocoder() { ApiKey = "my-api-key" };
            //var addresses = await geocoder.ReverseGeocodeAsync(49.525675, 31.73423);

            //Console.WriteLine(addresses.First().FormattedAddress);


            var host = CreateHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Getting things ready...");

            var mediator = host.Services.GetRequiredService<IMediator>();

            var user = await mediator.Send(new GetByIdQuery<User>(2));
            Console.WriteLine(user.Name);


            //var controller = host.Services.GetRequiredService<SimpleController>();

            //Console.Write("Enter username: ");
            //var username = Console.ReadLine();

            //var user = new User(username);
            //await controller.AddItemAsync(user);

            //var category = new Category("Important");
            //var category2 = new Category("Unimportant");

            //await controller.AddItemAsync(category);
            //await controller.AddItemAsync(category2);

            //var image = new Image("TrilliumLake_ROW5581315564_1920x1080", @"C:\Users\idims\Pictures\Фото Bing\TrilliumLake_ROW5581315564_1920x1080");
            //await controller.AddItemAsync(image);

            //var status = new Status("InProgress");
            //await controller.AddItemAsync(status);

            //Console.Write("Enter name of list: ");
            //var listName = Console.ReadLine();

            //var list = new Checklist(listName, user.Id);
            //await controller.AddItemAsync(list);

            //Console.Write("Enter task name: ");
            //var taskName = Console.ReadLine();

            //var dueTo = new DateTime();

            //var task = new ChecklistItem(taskName, list.Id, dueTo, category2.Id, null, image.Id, status.Id);
            //await controller.AddItemAsync(task);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
         Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddInfrastructure(context.Configuration.GetConnectionString("DefaultConnection"));
                services.AddTransient<SimpleController>();
                services.AddMediatR(Assembly.GetExecutingAssembly());
            })
            .ConfigureLogging(config =>
            {
                config.ClearProviders();
                config.AddConsole();
            });
    }
}
