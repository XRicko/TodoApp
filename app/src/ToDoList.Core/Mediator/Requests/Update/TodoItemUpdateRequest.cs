using System;
using System.Diagnostics.CodeAnalysis;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Requests.Update
{
    [ExcludeFromCodeCoverage]
    public record TodoItemUpdateRequest(int Id, string Name, int ChecklistId, int StatusId, DateTime StartDate, DateTime? DueDate = null, GeoCoordinate GeoPoint = null, int? ParentId = null, int? CategoryId = null, int? ImageId = null) : BaseRequest(Name);
}
