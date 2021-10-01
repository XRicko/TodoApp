namespace ToDoList.Core.Mediator.Response
{
    public record ProjectResponse(int Id, string Name, int UserId) : BaseResponse(Id, Name);
}
