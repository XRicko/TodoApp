using System;
using System.Diagnostics.CodeAnalysis;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Response
{
    [ExcludeFromCodeCoverage]
    public record TodoItemResponse(int Id, string Name, DateTime StartDate, int ChecklistId, string ChecklistName, int StatusId, string StatusName, DateTime? DueDate = null, int? CategoryId = null, string CategoryName = null, int? ImageId = null, string ImageName = null, string ImagePath = null, CategoryResponse Category = null) : BaseResponse(Id, Name)
    {
        public string Address { get; init; }
    }
}
