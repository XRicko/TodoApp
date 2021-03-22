using System;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Requests.Create
{
    public record TodoItemCreateRequest(string Name, int ChecklistId, DateTime? DueDate, GeoCoordinate GeoPoint, int? ParentId, int StatusId, int? CategoryId, int? ImageId) : BaseRequest(Name);
}
