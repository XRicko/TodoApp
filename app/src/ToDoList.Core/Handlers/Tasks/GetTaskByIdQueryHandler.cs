
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Tasks
{
    class GetTaskByIdQueryHandler : GetByIdQueryHandler<ChecklistItem>
    {
        public GetTaskByIdQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
