using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.TodoItems
{
    internal class RemoveTodoItemsCommandHandler : RemoveCommandHandler<TodoItem>
    {
        public RemoveTodoItemsCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
