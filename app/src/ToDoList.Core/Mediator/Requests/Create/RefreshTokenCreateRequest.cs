namespace ToDoList.Core.Mediator.Requests.Create
{
    public record RefreshTokenCreateRequest(string Name, int UserId) : BaseRequest(Name);
}
