namespace ToDoList.Core.Mediator.Response
{
    public record StatusResponse(int Id, string Name) : BaseResponse(Id, Name);
}
