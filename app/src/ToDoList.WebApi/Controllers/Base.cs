using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace ToDoList.WebApi.Controllers
{
    public abstract class Base : ControllerBase
    {
        protected readonly IMediator mediator;
        protected readonly IMapper mapper;

        protected Base(IMediator mediator1, IMapper mapper1)
        {
            mediator = mediator1;
            mapper = mapper1;
        }
    }
}
