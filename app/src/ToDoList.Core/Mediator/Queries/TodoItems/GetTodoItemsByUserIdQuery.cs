using System.Collections.Generic;

using MediatR;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Mediator.Queries.TodoItems
{
    public class GetTodoItemsByUserIdQuery : IRequest<IEnumerable<TodoItemResponse>>
    {
        public int UserId { get; }

        public GetTodoItemsByUserIdQuery(int userId)
        {
            UserId = userId;
        }
    }
}
