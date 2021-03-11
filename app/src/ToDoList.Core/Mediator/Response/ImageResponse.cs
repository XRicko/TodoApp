namespace ToDoList.Core.Mediator.Response
{
    public record ImageResponse(int Id, string Name, string Path) : BaseResponse(Id, Name);
}
