using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using ToDoList.Infrastructure.Data;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Infrastructure.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<TodoListContext>(options => options.UseSqlServer(connectionString, x => x.UseNetTopologySuite()));

            services.AddScoped<IRepository, EfRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
