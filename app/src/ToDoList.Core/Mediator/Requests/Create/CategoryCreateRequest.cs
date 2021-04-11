using System.Diagnostics.CodeAnalysis;

namespace ToDoList.Core.Mediator.Requests.Create
{
    [ExcludeFromCodeCoverage]
    public record CategoryCreateRequest(string Name) : BaseRequest(Name);
}
