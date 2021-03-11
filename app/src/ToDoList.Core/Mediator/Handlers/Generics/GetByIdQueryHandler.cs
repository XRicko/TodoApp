using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Mediator.Queries;
using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Generics
{
    internal class GetByIdQueryHandler<TEntity, TResponse> : HandlerBase, IRequestHandler<GetByIdQuery<TEntity, TResponse>, TResponse>
        where TEntity : BaseEntity
        where TResponse : BaseResponse
    {
        public GetByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        public virtual async Task<TResponse> Handle(GetByIdQuery<TEntity, TResponse> request, CancellationToken cancellationToken)
        {
            var entity = await UnitOfWork.Repository.GetAsync<TEntity>(request.Id);
            var response = Mapper.Map<TResponse>(entity);

            return response;
        }
    }
}
