namespace ToDoList.Core.Mediator.Response
{
    public record CategoryResponse(int Id, string Name) : BaseResponse(Id, Name);
}
