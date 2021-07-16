using AutoMapper;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Users
{
    internal class GetUserByNameQueryHandler : GetByNameQueryHandler<User, UserResponse>
    {
        public GetUserByNameQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
