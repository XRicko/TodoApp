namespace ToDoList.SharedKernel
{
    public record GeoCoordinate
    {
        public const int SRID = 4326;

        public double Longitude { get; init; }
        public double Latitude { get; init; }

        public GeoCoordinate()
        {

        }

        public GeoCoordinate(double longitude, double latitude)
            => (Longitude, Latitude) = (longitude, latitude);

        public void Deconstruct(out double longitude, out double latitude)
            => (longitude, latitude) = (Longitude, Latitude);
    }
}
