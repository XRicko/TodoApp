using System;

using Geocoding;
using Geocoding.Google;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.Core.Services;
using ToDoList.Extensions;

namespace ToDoList.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
        {
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var apiKeys = configuration.GetSection(ApiKeys.Apis).GetValid<ApiKeys>();
            services.AddSingleton(apiKeys);

            services.AddGoogleGeocoder(apiKeys.GoogleApiKey);

            services.AddTransient<IGeocodingService, GeocodingService>();
            services.AddTransient<IAddressService, AddressService>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddAutoMapper(typeof(CategoryResponse));
            services.AddMediatR(typeof(GetAllQuery<,>));

            return services;
        }

        private static IServiceCollection AddGoogleGeocoder(this IServiceCollection services, string apiKey) =>
            services.AddScoped<IGeocoder>(x => new GoogleGeocoder(apiKey));
    }
}
