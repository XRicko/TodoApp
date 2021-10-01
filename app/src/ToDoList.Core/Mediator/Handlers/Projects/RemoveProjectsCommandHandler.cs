using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Entities;
using ToDoList.Core.Extensions;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Projects
{
    internal class RemoveProjectsCommandHandler : RemoveCommandHandler<Project>
    {
        public RemoveProjectsCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public override async Task<Unit> Handle(RemoveCommand<Project> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var project = UnitOfWork.Repository.GetAll<Project>()
                                               .Select(x => new Project
                                               {
                                                   Id = x.Id,
                                                   Name = x.Name,
                                                   Checklists = x.Checklists.Select(c => new Checklist
                                                   {
                                                       Id = c.Id,
                                                       ProjectId = c.ProjectId
                                                   }).ToList()
                                               })
                                               .SingleOrDefault(x => x.Id == request.Id);

            if (project is not null && project.Name != "Untitled")
            {
                foreach (var checklist in project.Checklists)
                {
                    UnitOfWork.Repository.RemoveChecklist(checklist.Id);
                }
                UnitOfWork.Repository.Remove(project);

                await UnitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
