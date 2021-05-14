using MediatR;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Mediator.Commands.Generics
{
    public record RemoveCommand<TEntity>(int Id) : IRequest
        where TEntity : BaseEntity;
}
