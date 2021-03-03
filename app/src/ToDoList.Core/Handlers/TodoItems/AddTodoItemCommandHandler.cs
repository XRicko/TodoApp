using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.TodoItems
{
    internal class AddTodoItemCommandHandler : AddCommandHandler<TodoItem>
    {
        public AddTodoItemCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
