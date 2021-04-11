using System.Diagnostics.CodeAnalysis;

namespace ToDoList.Core.Mediator.Response
{
    [ExcludeFromCodeCoverage]
    public record StatusResponse(int Id, string Name, bool IsDone) : BaseResponse(Id, Name);
}
