namespace ToDoList.Core.Mediator.Response
{
    public record RefreshTokenResponse(int Id, string Name, int UserId) : BaseResponse(Id, Name);
}
