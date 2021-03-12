using Geocoding;
using Geocoding.Google;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ToDoList.WebApi.Extensions
{
    public static class ServiceCollectionForIGeocoderExtensions
    {
        public static IServiceCollection AddGoogleGeocoder(this IServiceCollection services) =>
            services.AddScoped<IGeocoder>(x => new GoogleGeocoder(x.GetService<IOptions<ApiOptions>>().Value.GoogleApiKey));
    }
}
