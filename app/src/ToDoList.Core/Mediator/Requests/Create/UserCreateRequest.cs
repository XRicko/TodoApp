namespace ToDoList.Core.Mediator.Requests.Create
{
    public record UserCreateRequest(string Name) : BaseRequest(Name);
}
