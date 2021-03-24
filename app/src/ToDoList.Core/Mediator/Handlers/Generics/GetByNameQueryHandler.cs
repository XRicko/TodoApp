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
    public class GetByNameQueryHandler<TEntity, TResponse> : HandlerBase, IRequestHandler<GetByNameQuery<TEntity, TResponse>, TResponse>
        where TEntity : BaseEntity
        where TResponse : BaseResponse
    {
        public GetByNameQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }

        public async Task<TResponse> Handle(GetByNameQuery<TEntity, TResponse> request, CancellationToken cancellationToken)
        {
            var entity = await UnitOfWork.Repository.GetAsync<TEntity>(request.Name);
            var response = Mapper.Map<TResponse>(entity);

            return response;
        }
    }
}
