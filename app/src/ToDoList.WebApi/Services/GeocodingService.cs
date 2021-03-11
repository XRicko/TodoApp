using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Geocoding;

namespace ToDoList.WebApi.Services
{
    public class GeocodingService
    {
        private readonly IGeocoder geocoder;

        public GeocodingService(IGeocoder coder)
        {
            geocoder = coder;
        }

        public async Task<string> GetAddressAsync(double latitude, double longitude)
        {
            IEnumerable<Address> addresses = await geocoder.ReverseGeocodeAsync(latitude, longitude);
            return addresses.First().FormattedAddress;
        }
    }
}
