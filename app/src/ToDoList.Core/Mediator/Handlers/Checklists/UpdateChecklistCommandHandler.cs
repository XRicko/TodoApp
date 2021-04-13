using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Checklists
{
    [ExcludeFromCodeCoverage]
    public class UpdateChecklistCommandHandler : UpdateCommandHandler<ChecklistUpdateRequest, Checklist>
    {
        public UpdateChecklistCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }

        public override async Task<Unit> Handle(UpdateCommand<ChecklistUpdateRequest> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            await base.Handle(request, cancellationToken);

            var checklists = await UnitOfWork.Repository.GetAllAsync<Checklist>();
            var defaultChecklist = checklists.SingleOrDefault(x => x.Name == "Untitled"
                                                                   && x.UserId == request.Request.UserId);

            if (defaultChecklist is null)
            {
                await UnitOfWork.Repository.AddAsync(new Checklist { Name = "Untitled", UserId = request.Request.UserId });
                await UnitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
