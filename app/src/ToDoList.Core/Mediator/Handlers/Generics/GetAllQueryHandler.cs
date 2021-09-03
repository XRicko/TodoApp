using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Generics
{
    internal abstract class GetAllQueryHandler<TEntity, TResponse> : HandlerBase, IRequestHandler<GetAllQuery<TEntity, TResponse>, IEnumerable<TResponse>>
        where TEntity : BaseEntity
        where TResponse : BaseResponse
    {
        protected GetAllQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        public virtual async Task<IEnumerable<TResponse>> Handle(GetAllQuery<TEntity, TResponse> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            return await Task.FromResult(UnitOfWork.Repository.GetAll<TEntity>()
                                                              .ProjectTo<TResponse>(Mapper.ConfigurationProvider)
                                                              .ToList());
        }
    }
}
