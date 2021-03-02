using System;

namespace ToDoList.Core.DTOs
{
    public record ChecklistItemDTO(int Id, string Name, DateTime StartDate, DateTime? DueDate, GeoCoordinate GeoPoint);
}
