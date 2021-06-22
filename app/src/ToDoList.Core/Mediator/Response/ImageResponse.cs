namespace ToDoList.Core.Mediator.Response
{
    public record ImageResponse(int Id, string Name, byte[] Content) : BaseResponse(Id, Name);
}
