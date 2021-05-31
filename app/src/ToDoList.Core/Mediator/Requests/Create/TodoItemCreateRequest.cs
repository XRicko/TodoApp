using System;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Requests.Create
{
    public record TodoItemCreateRequest(string Name, int ChecklistId, int StatusId, DateTime? DueDate = null, GeoCoordinate GeoPoint = null, int? CategoryId = null, int? ImageId = null) : BaseRequest(Name);
}
