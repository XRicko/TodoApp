using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace ToDoList.WebApi.Controllers
{
    public abstract class Base : ControllerBase
    {
        protected IMediator Mediator { get; }
        protected IMapper Mapper { get; }

        protected Base(IMediator mediator, IMapper mapper)
        {
            Mediator = mediator;
            Mapper = mapper;
        }
    }
}
