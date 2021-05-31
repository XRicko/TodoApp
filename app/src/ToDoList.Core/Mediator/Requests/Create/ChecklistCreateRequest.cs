namespace ToDoList.Core.Mediator.Requests.Create
{
    public record ChecklistCreateRequest(string Name, int UserId) : BaseRequest(Name);
}
