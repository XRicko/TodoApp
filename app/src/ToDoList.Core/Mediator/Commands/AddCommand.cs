using MediatR;

using ToDoList.Core.Mediator.Requests;

namespace ToDoList.Core.Mediator.Commands
{
    public class AddCommand<TRequest> : IRequest
        where TRequest : BaseRequest
    {
        public TRequest Request { get; }

        public AddCommand(TRequest entity)
        {
            Request = entity ?? throw new System.ArgumentNullException(nameof(entity));
        }
    }
}
