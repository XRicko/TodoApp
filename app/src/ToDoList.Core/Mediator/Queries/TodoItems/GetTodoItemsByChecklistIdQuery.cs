using System.Collections.Generic;

using MediatR;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Mediator.Queries.TodoItems
{
    public record GetTodoItemsByChecklistIdQuery(int ChecklistId) : IRequest<IEnumerable<TodoItemResponse>>;
}
