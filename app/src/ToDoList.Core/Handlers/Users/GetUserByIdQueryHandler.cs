
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Users
{
    internal class GetUserByIdQueryHandler : GetByIdQueryHandler<User>
    {
        public GetUserByIdQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
