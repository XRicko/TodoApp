namespace ToDoList.Core.Mediator.Response
{
    public record UserResponse(int Id, string Name) : BaseResponse(Id, Name);
}
