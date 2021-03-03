
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Statuses
{
    class GetStatusByIdQueryHandler : GetByIdQueryHandler<Status>
    {
        public GetStatusByIdQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
