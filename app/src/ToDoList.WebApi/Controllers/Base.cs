
using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace ToDoList.WebApi.Controllers
{
    public abstract class Base : ControllerBase
    {
        protected IMediator Mediator { get; }

        protected Base(IMediator mediator)
        {
            Mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }
    }
}
