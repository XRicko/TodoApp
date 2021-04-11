using System;
using System.Diagnostics.CodeAnalysis;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Requests.Create
{
    [ExcludeFromCodeCoverage]
    public record TodoItemCreateRequest(string Name, int ChecklistId, int StatusId, DateTime? DueDate = null, GeoCoordinate GeoPoint = null, int? ParentId = null, int? CategoryId = null, int? ImageId = null) : BaseRequest(Name);
}
