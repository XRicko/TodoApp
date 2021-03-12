
using AutoMapper;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Users
{
    class GetUserByIdQueryHandler : GetByIdQueryHandler<User, UserResponse>
    {
        public GetUserByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }
    }
}
