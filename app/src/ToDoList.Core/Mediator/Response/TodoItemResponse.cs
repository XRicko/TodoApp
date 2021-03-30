using System;
using System.Diagnostics.CodeAnalysis;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Response
{
    [ExcludeFromCodeCoverage]
    public record TodoItemResponse(int Id, string Name, DateTime StartDate, int ChecklistId, string ChecklistName, int StatusId, string StatusName, DateTime? DueDate = null, GeoCoordinate GeoPoint = null, int? ParentId = null, int? CategoryId = null, string CategoryName = null, int? ImageId = null, string ImagePath = null, string Address = null) : BaseResponse(Id, Name);
}
