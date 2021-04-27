using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Generics
{
    public class GetByIdQueryHandler<TEntity, TResponse> : HandlerBase, IRequestHandler<GetByIdQuery<TEntity, TResponse>, TResponse>
        where TEntity : BaseEntity
        where TResponse : BaseResponse
    {
        public GetByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        public virtual async Task<TResponse> Handle(GetByIdQuery<TEntity, TResponse> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var entity = await UnitOfWork.Repository.FindByPrimaryKeysAsync<TEntity>(request.Id);
            var response = Mapper.Map<TResponse>(entity);

            return response;
        }
    }
}
