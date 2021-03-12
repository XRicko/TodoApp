using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Geocoding;
using Geocoding.Google;

namespace ToDoList.Core.Services
{
    public class GoogleGeocodingService : IGeocodingService
    {
        private readonly GoogleGeocoder googleGeocoder;
        private readonly ApiOptions apiOptions;

        public GoogleGeocodingService(ApiOptions options)
        {
            apiOptions = options;
            googleGeocoder = new GoogleGeocoder(apiOptions.GoogleApiKey);
        }

        public async Task<string> GetAddressAsync(double latitude, double longitude)
        {
            IEnumerable<Address> addresses = await googleGeocoder.ReverseGeocodeAsync(latitude, longitude);
            return addresses.First().FormattedAddress;
        }
    }
}
