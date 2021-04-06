using MediatR;

using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Queries.Generics
{
    public class GetByIdQuery<TEntity, TResponse> : IRequest<TResponse>
        where TEntity : BaseEntity
        where TResponse : BaseResponse
    {
        public int Id { get; }

        public GetByIdQuery(int id)
        {
            Id = id;
        }
    }
}
