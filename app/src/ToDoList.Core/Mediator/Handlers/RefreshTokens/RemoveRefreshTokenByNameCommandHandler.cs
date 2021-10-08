using AutoMapper;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.RefreshTokens
{
    internal class RemoveRefreshTokenByNameCommandHandler : RemoveByNameCommandHandler<RefreshToken>
    {
        public RemoveRefreshTokenByNameCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
