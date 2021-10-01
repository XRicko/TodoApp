using AutoMapper;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Projects
{
    internal class UpdateProjectCommandHanlder : UpdateCommandHandler<ProjectUpdateRequest, Project>
    {
        public UpdateProjectCommandHanlder(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
