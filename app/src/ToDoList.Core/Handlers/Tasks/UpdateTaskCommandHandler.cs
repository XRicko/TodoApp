
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Tasks
{
    class UpdateTaskCommandHandler : UpdateCommandHandler<ChecklistItem>
    {
        public UpdateTaskCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
