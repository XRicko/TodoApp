using System.Collections.Generic;

using MediatR;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Mediator.Queries.TodoItems
{
    public class GetActiveOrDoneTodoItemsByUserQuery : IRequest<IEnumerable<TodoItemResponse>>
    {
        public int UserId { get; }
        public bool IsDone { get; set; }

        public GetActiveOrDoneTodoItemsByUserQuery(int userId, bool isDone)
        {
            UserId = userId;
            IsDone = isDone;
        }
    }
}
