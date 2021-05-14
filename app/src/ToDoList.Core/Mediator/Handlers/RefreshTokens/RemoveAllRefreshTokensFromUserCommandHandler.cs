using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.RefreshTokens;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.RefreshTokens
{
    internal class RemoveAllRefreshTokensFromUserCommandHandler : HandlerBase, IRequestHandler<RemoveAllRefreshTokensFromUserCommand>
    {
        public RemoveAllRefreshTokensFromUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<Unit> Handle(RemoveAllRefreshTokensFromUserCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var refreshTokens = UnitOfWork.Repository.GetAll<RefreshToken>()
                                                     .Where(x => x.UserId == request.UserId)
                                                     .Select(x => new RefreshToken { Id = x.Id })
                                                     .ToList();

            foreach (var refreshToken in refreshTokens)
            {
                UnitOfWork.Repository.Remove(refreshToken);
            }
            await UnitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
