using System;

using ToDoList.SharedKernel;

namespace ToDoList.WebApi.Requests.Update
{
    public record TodoItemUpdateRequest(int Id, string Name, DateTime? DueDate, GeoCoordinate GeoPoint, int? ParentId, int? StatusId, int? CategoryId, int ChecklistId, int? ImageId);
}
