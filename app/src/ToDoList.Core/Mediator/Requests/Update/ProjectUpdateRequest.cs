namespace ToDoList.Core.Mediator.Requests.Update
{
    public record ProjectUpdateRequest(int Id, string Name, int UserId) : BaseRequest(Name);
}
