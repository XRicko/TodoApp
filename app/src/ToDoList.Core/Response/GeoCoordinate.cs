namespace ToDoList.Core.Response
{
    public record GeoCoordinate(double Longitude, double Latitude, int SRID = 4326);
}
