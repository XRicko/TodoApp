using System.Diagnostics.CodeAnalysis;

namespace ToDoList.Core.Mediator.Requests.Create
{
    [ExcludeFromCodeCoverage]
    public record ChecklistCreateRequest(string Name, int UserId) : BaseRequest(Name);
}
