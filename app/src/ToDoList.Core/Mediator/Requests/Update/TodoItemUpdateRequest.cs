using System;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Requests.Update
{
    public record TodoItemUpdateRequest(int Id, string Name, DateTime StartDate, DateTime? DueDate, GeoCoordinate GeoPoint, int? ParentId, int? StatusId, int? CategoryId, int ChecklistId, int? ImageId) : BaseRequest(Name);
}
