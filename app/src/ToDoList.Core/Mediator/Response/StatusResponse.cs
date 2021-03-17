namespace ToDoList.Core.Mediator.Response
{
    public record StatusResponse(int Id, string Name, bool IsDone) : BaseResponse(Id, Name);
}
