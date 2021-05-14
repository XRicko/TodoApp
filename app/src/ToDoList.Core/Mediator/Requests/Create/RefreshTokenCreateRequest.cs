using System.Diagnostics.CodeAnalysis;

namespace ToDoList.Core.Mediator.Requests.Create
{
    [ExcludeFromCodeCoverage]
    public record RefreshTokenCreateRequest(string Name, int UserId) : BaseRequest(Name);
}
