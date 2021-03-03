
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Statuses
{
    class GetStatusesQueryHandler : GetAllQueryHandler<Status>
    {
        public GetStatusesQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
