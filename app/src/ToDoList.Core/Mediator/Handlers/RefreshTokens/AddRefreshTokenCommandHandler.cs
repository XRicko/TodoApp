using AutoMapper;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.RefreshTokens
{
    internal class AddRefreshTokenCommandHandler : AddCommandHandler<RefreshTokenCreateRequest, RefreshToken>
    {
        public AddRefreshTokenCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
