using System.Collections.Generic;

using MediatR;

using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Queries.Generics
{
    public class GetAllQuery<TEntity, TResponse> : IRequest<IEnumerable<TResponse>>
        where TEntity : BaseEntity
        where TResponse : BaseResponse
    {

    }
}
