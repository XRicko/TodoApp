using System;

using ToDoList.Core.Response;

namespace ToDoList.WebApi.Requests.Update
{
    public record TodoItemUpdateRequest(int Id, string Name, DateTime StartDate, DateTime? DueDate, GeoCoordinate GeoPoint, int? ParentId, int? StatusId, int? CategoryId, int ChecklistId, int? ImageId);
}
