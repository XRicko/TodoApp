using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Queries.RefreshTokens;
using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.RefreshTokens
{
    internal class GetRefreshTokenByTokenQueryHandler : HandlerBase, IRequestHandler<GetRefreshTokenByTokenQuery, RefreshTokenResponse>
    {
        public GetRefreshTokenByTokenQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public Task<RefreshTokenResponse> Handle(GetRefreshTokenByTokenQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var refreshToken = UnitOfWork.Repository.GetAll<RefreshToken>()
                                                    .FirstOrDefault(x => x.Name == request.Token);

            return Task.FromResult(Mapper.Map<RefreshTokenResponse>(refreshToken));
        }
    }
}
