namespace ToDoList.Core.Mediator.Requests.Update
{
    public record ChecklistUpdateRequest(int Id, string Name, int UserId) : BaseRequest(Name);
}
