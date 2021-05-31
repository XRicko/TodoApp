namespace ToDoList.Core.Mediator.Response
{
    public record UserResponse(int Id, string Name, string Password) : BaseResponse(Id, Name);
}
