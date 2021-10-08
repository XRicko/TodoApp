using MediatR;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Commands.Generics
{
    public record RemoveByNameCommand<TEntity>(string Name) : IRequest
        where TEntity : BaseEntity;
}
