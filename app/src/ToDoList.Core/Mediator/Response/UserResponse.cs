using System.Diagnostics.CodeAnalysis;

namespace ToDoList.Core.Mediator.Response
{
    [ExcludeFromCodeCoverage]
    public record UserResponse(int Id, string Name, string Password) : BaseResponse(Id, Name);
}
