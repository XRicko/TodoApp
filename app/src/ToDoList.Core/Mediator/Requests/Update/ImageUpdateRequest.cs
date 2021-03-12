namespace ToDoList.Core.Mediator.Requests.Update
{
    public record ImageUpdateRequest(int Id, string Name, string Path) : BaseRequest(Name);
}
