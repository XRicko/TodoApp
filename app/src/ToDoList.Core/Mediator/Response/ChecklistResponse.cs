namespace ToDoList.Core.Mediator.Response
{
    public record ChecklistResponse(int Id, string Name, int ProjectId) : BaseResponse(Id, Name);
}
