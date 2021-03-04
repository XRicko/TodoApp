using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Geocoding;
using Geocoding.Google;

namespace ToDoList.Core.Response
{
    public record TodoItemResponse(int Id, string Name, DateTime StartDate, DateTime? DueDate, GeoCoordinate GeoPoint, int? ParentId, string StatusName, string CategoryName, string ChecklistName, string ImagePath, string Address = null)
    {
        public string Address 
        {
            get
            {
                IGeocoder geocoder = new GoogleGeocoder(Credentials.GoogleApiKey);
                var addresses = geocoder.ReverseGeocodeAsync(GeoPoint.Latitude, GeoPoint.Longitude);

                return addresses.Result.First().FormattedAddress;
            }
        }
    }
}
