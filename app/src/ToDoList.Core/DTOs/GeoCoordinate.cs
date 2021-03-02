namespace ToDoList.Core.DTOs
{
    public record GeoCoordinate(double Longitude, double Latitude, int SRID = 4326);
}
