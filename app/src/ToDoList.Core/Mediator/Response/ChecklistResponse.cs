namespace ToDoList.Core.Mediator.Response
{
    public record ChecklistResponse(int Id, string Name, string UserName) : BaseResponse(Id, Name);
}
