namespace ToDoList.Core.Mediator.Requests.Create
{
    public record ChecklistCreateRequest(string Name, int ProjectId) : BaseRequest(Name);
}
