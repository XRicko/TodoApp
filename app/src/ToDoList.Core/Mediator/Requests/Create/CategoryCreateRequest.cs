namespace ToDoList.Core.Mediator.Requests.Create
{
    public record CategoryCreateRequest(string Name) : BaseRequest(Name);
}
