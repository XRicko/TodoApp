
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Users
{
    class GetUsersQueryHandler : GetAllQueryHandler<User>
    {
        public GetUsersQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
