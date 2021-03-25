using System.Diagnostics.CodeAnalysis;

namespace ToDoList.Core.Mediator.Requests.Create
{
    [ExcludeFromCodeCoverage]
    public record ImageCreateRequest(string Name, string Path) : BaseRequest(Name);
}
