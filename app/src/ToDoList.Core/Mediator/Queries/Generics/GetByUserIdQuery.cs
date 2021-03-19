using System.Collections.Generic;

using MediatR;

using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Queries.Generics
{
    public class GetByUserIdQuery<TEntity, TResponse> : IRequest<IEnumerable<TResponse>>
        where TEntity : BaseEntity
        where TResponse : BaseResponse
    {
        public int UserId { get; }

        public GetByUserIdQuery(int userId)
        {
            UserId = userId;
        }
    }
}
