using AutoMapper;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.RefreshTokens
{
    internal class RemoveRefreshTokenCommandHandler : RemoveCommandHandler<RefreshToken>
    {
        public RemoveRefreshTokenCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
