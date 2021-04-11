using System.Diagnostics.CodeAnalysis;

namespace ToDoList.Core.Mediator.Requests
{
    [ExcludeFromCodeCoverage]
    public record UserRequest(string Name, string Password) : BaseRequest(Name);
}
