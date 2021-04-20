using System.Collections.Generic;

using MediatR;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Mediator.Queries.TodoItems
{
    public record GetTodoItemsByUserIdQuery(int UserId) : IRequest<IEnumerable<TodoItemResponse>>;
}
