using System.Diagnostics.CodeAnalysis;

namespace ToDoList.Core.Mediator.Response
{
    [ExcludeFromCodeCoverage]
    public abstract record BaseResponse(int Id, string Name);
}
