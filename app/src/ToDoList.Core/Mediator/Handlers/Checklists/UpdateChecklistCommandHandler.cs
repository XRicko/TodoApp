using System.Diagnostics.CodeAnalysis;

using AutoMapper;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Checklists
{
    public class UpdateChecklistCommandHandler : UpdateCommandHandler<ChecklistUpdateRequest, Checklist>
    {
        [ExcludeFromCodeCoverage]
        public UpdateChecklistCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }
    }
}
