namespace ToDoList.Core.Mediator.Requests.Create
{
    public record UserCreateRequest(string Name, string Password) : BaseRequest(Name);
}
