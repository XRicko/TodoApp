using System.Diagnostics.CodeAnalysis;

namespace ToDoList.Core.Mediator.Response
{
    [ExcludeFromCodeCoverage]
    public record ChecklistResponse(int Id, string Name, int UserId) : BaseResponse(Id, Name);
}
