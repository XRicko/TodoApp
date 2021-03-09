using System;

using ToDoList.Core.Response;

namespace ToDoList.WebApi.Requests.Create
{
    public record TodoItemCreateRequest(string Name, DateTime? DueDate, GeoCoordinate GeoPoint, int? ParentId, int? StatusId, int? CategoryId, int ChecklistId, int? ImageId);
}
