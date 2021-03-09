using System;
using System.Linq;

using Geocoding;
using Geocoding.Google;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Response
{
    public record TodoItemResponse(int Id, string Name, DateTime StartDate, DateTime? DueDate, GeoCoordinate GeoPoint, int? ParentId, string StatusName, string CategoryName, string ChecklistName, string ImagePath, string Address = null)
    {
        public string Address
        {
            get
            {
                if (GeoPoint is not null)
                {
                    IGeocoder geocoder = new GoogleGeocoder(Credentials.GoogleApiKey);
                    var addresses = geocoder.ReverseGeocodeAsync(GeoPoint.Latitude, GeoPoint.Longitude);

                    return addresses.Result.FirstOrDefault()?.FormattedAddress;
                }

                return null;
            }
        }
    }
}
