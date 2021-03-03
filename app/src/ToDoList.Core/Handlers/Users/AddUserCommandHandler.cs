
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Users
{
    class AddUserCommandHandler : AddCommandHandler<User>
    {
        public AddUserCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
