
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.TodoItems
{
    internal class GetTodoItemByIdQueryHandler : GetByIdQueryHandler<TodoItem>
    {
        public GetTodoItemByIdQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
