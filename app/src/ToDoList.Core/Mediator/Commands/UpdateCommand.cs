using MediatR;

using ToDoList.Core.Mediator.Requests;

namespace ToDoList.Core.Mediator.Commands
{
    public record UpdateCommand<TRequest>(TRequest Request) : IRequest 
        where TRequest : BaseRequest;
}
