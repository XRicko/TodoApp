using MediatR;

using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Queries.Generics
{
    public record GetByNameQuery<TEntity, TResponse>(string Name) : IRequest<TResponse>
        where TEntity : BaseEntity
        where TResponse : BaseResponse;
}
