using System;

namespace ToDoList.Core.Response
{
    public record TodoItemResponse(int Id, string Name, DateTime StartDate, DateTime? DueDate, GeoCoordinate GeoPoint, int? ParentId, string StatusName, string CategoryName, string ChecklistName, string ImagePath, string Address = null);
}
