using AutoMapper;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Images
{
    class AddImageCommandHandler : AddCommandHandler<ImageCreateRequest, Image>
    {
        public AddImageCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }
    }
}
