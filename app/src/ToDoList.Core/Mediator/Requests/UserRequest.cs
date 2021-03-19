namespace ToDoList.Core.Mediator.Requests
{
    public record UserRequest(string Name, string Password) : BaseRequest(Name);
}
