namespace ToDoList.Core.Mediator.Requests.Create
{
    public record ImageCreateRequest(string Name, string Path) : BaseRequest(Name);
}
