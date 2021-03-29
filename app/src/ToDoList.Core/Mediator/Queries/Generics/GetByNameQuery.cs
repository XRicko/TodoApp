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
            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentException($"'{nameof(name)}' cannot be null or empty", nameof(name));

            Name = name;
        }
    }
}
