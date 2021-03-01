using System;

using NetTopologySuite.Geometries;

namespace ToDoList.Core.DTOs
{
    public record ChecklistItemDTO(int Id, string Name, DateTime StartDate, DateTime? DueDate, Point GeoPoint);
}
