using MediatR;

namespace ToDoList.Core.Controllers
{
    public abstract class ControllerBase
    {
        protected IMediator Mediator { get; }

        public ControllerBase(IMediator mediator)
        {
            Mediator = mediator;
        }
    }
}
