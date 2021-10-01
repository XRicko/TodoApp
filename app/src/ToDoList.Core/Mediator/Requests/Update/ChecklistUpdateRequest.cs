namespace ToDoList.Core.Mediator.Requests.Update
{
    public record ChecklistUpdateRequest(int Id, string Name, int ProjectId) : BaseRequest(Name);
}
