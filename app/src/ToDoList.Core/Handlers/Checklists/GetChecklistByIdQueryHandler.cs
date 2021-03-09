
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Checklists
{
    internal class GetChecklistByIdQueryHandler : GetByIdQueryHandler<Checklist>
    {
        public GetChecklistByIdQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
