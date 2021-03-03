using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.TodoItems
{
    internal class GetTodoItemsQueryHandler : GetAllQueryHandler<TodoItem>
    {
        public GetTodoItemsQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
