using System.Diagnostics.CodeAnalysis;

namespace ToDoList.SharedKernel
{
    [ExcludeFromCodeCoverage]
    public record GeoCoordinate(double Longitude, double Latitude);
}
