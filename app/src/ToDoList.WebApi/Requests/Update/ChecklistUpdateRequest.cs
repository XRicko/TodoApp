namespace ToDoList.WebApi.Requests.Update
{
    public record ChecklistUpdateRequest(int Id, string Name, int UserId);
}
