
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Checklists
{
    class GetChecklistsQueryHandler : GetAllQueryHandler<Checklist>
    {
        public GetChecklistsQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
