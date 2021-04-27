using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Checklists
{
    public class AddChecklistCommandHandler : AddCommandHandler<ChecklistCreateRequest, Checklist>
    {
        public AddChecklistCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }

        public override async Task<Unit> Handle(AddCommand<ChecklistCreateRequest> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var checklist = UnitOfWork.Repository.GetAll<Checklist>()
                                                 .SingleOrDefault(e => e.Name == request.Request.Name
                                                                       && e.UserId == request.Request.UserId);

            if (checklist is null)
            {
                var entity = Mapper.Map<Checklist>(request.Request);

                await UnitOfWork.Repository.AddAsync(entity);
                await UnitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
