using MediatR;

using ToDoList.Core.Mediator.Requests;

namespace ToDoList.Core.Mediator.Commands.Generics
{
    public record AddCommand<TRequest>(TRequest Request) : IRequest
        where TRequest : BaseRequest;
}
