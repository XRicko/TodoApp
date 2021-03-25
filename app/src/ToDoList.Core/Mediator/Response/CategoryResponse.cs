using System.Diagnostics.CodeAnalysis;

namespace ToDoList.Core.Mediator.Response
{
    [ExcludeFromCodeCoverage]
    public record CategoryResponse(int Id, string Name) : BaseResponse(Id, Name);
}
