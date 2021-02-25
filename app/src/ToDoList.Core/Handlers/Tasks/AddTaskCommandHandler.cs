using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Tasks
{
    class AddTaskCommandHandler : AddCommandHandler<ChecklistItem>
    {
        public AddTaskCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
