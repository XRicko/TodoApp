
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Checklists
{
    class UpdateChecklistCommandHandler : UpdateCommandHandler<Checklist>
    {
        public UpdateChecklistCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
