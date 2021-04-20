using MediatR;

using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Queries.Generics
{
    public record GetByIdQuery<TEntity, TResponse>(int Id) : IRequest<TResponse>
        where TEntity : BaseEntity
        where TResponse : BaseResponse;
}
