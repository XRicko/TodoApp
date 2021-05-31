using System;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Requests.Update
{
    public record TodoItemUpdateRequest(int Id, string Name, int ChecklistId, int StatusId, DateTime StartDate, DateTime? DueDate = null, GeoCoordinate GeoPoint = null, int? CategoryId = null, int? ImageId = null) : BaseRequest(Name);
}
