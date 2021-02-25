
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Tasks
{
    class RemoveTaskCommandHandler : RemoveCommandHandler<ChecklistItem>
    {
        public RemoveTaskCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
