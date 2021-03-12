using AutoMapper;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Checklists
{
    internal class RemoveChecklistCommandHandler : RemoveCommandHandler<Checklist>
    {
        public RemoveChecklistCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }
    }
}
