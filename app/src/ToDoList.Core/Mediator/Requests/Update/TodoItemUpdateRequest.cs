using System;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Requests.Update
{
    public record TodoItemUpdateRequest(int Id, string Name, int ChecklistId, DateTime StartDate, DateTime? DueDate, GeoCoordinate GeoPoint, int? ParentId, int StatusId, int? CategoryId, int? ImageId) : BaseRequest(Name);
}
