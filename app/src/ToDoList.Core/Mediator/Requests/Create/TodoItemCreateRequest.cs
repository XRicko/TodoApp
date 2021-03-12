using System;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Mediator.Requests.Create
{
    public record TodoItemCreateRequest(string Name, DateTime StartDate, DateTime? DueDate, GeoCoordinate GeoPoint, int? ParentId, int? StatusId, int? CategoryId, int ChecklistId, int? ImageId) : BaseRequest(Name);
}
