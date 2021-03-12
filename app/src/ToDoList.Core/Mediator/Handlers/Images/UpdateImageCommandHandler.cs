using AutoMapper;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Images
{
    class UpdateImageCommandHandler : UpdateCommandHandler<ImageUpdateRequest, Image>
    {
        public UpdateImageCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }
    }
}
