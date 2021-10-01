namespace ToDoList.Core.Mediator.Requests.Create
{
    public record ProjectCreateRequest(string Name, int UserId) : BaseRequest(Name);
}
