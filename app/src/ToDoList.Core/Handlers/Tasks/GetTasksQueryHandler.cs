
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Tasks
{
    internal class GetTasksQueryHandler : GetAllQueryHandler<ChecklistItem>
    {
        public GetTasksQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
