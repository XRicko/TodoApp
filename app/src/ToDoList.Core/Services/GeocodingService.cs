using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Geocoding;

namespace ToDoList.Core.Services
{
    internal class GeocodingService : IGeocodingService
    {
        private readonly IGeocoder geocoder;

        public GeocodingService(IGeocoder coder)
        {
            geocoder = coder ?? throw new System.ArgumentNullException(nameof(coder));
        }

        public async Task<string> GetAddressAsync(double latitude, double longitude)
        {
            IEnumerable<Address> addresses = await geocoder.ReverseGeocodeAsync(latitude, longitude);
            return addresses.First().FormattedAddress;
        }
    }
}
