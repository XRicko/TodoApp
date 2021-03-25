using System.Diagnostics.CodeAnalysis;

namespace ToDoList.Core.Mediator.Response
{
    [ExcludeFromCodeCoverage]
    public record ImageResponse(int Id, string Name, string Path) : BaseResponse(Id, Name);
}
