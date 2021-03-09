
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Checklists
{
    internal class AddChecklistCommandHandler : AddCommandHandler<Checklist>
    {
        public AddChecklistCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
