using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Checklists
{
    internal class AddChecklistCommandHandler : AddCommandHandler<ChecklistCreateRequest, Checklist>
    {
        public AddChecklistCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }

        public override async Task<Unit> Handle(AddCommand<ChecklistCreateRequest> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            bool recordExists = UnitOfWork.Repository.GetAll<Checklist>()
                                                     .Any(e => e.Name == request.Request.Name
                                                               && e.ProjectId == request.Request.ProjectId);

            if (!recordExists)
            {
                var entity = Mapper.Map<Checklist>(request.Request);

                UnitOfWork.Repository.Add(entity);
                await UnitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
