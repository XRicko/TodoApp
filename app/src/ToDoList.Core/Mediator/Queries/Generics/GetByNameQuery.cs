using MediatR;

using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Queries.Generics
{
    public class GetByNameQuery<TEntity, TResponse> : IRequest<TResponse>
        where TEntity : BaseEntity
        where TResponse : BaseResponse
    {
        public string Name { get; }

        public GetByNameQuery(string name)
        {
            Name = name;
        }
    }
}
