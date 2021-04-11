using System.Diagnostics.CodeAnalysis;

namespace ToDoList.Core.Mediator.Requests
{
    [ExcludeFromCodeCoverage]
    public abstract record BaseRequest(string Name);
}
