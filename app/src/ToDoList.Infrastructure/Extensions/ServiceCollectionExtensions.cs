using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using ToDoList.Infrastructure.Data;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ToDoListContext>(options => options.UseSqlServer(connectionString, x => x.UseNetTopologySuite())
                                                                 .UseLazyLoadingProxies());
            services.AddScoped<IRepository, EfRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
