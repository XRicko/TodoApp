using MediatR;

using ToDoList.Core.Mediator.Requests;

namespace ToDoList.Core.Mediator.Commands
{
    public class UpdateCommand<TRequest> : IRequest where TRequest : BaseRequest
    {
        public TRequest Request { get; set; }

        public UpdateCommand(TRequest request)
        {
            Request = request ?? throw new System.ArgumentNullException(nameof(request));
        }
    }
}
