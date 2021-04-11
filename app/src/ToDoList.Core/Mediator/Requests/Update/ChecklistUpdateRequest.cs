using System.Diagnostics.CodeAnalysis;

namespace ToDoList.Core.Mediator.Requests.Update
{
    [ExcludeFromCodeCoverage]
    public record ChecklistUpdateRequest(int Id, string Name, int UserId) : BaseRequest(Name);
}
