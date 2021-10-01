using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Entities;
using ToDoList.Core.Extensions;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Checklists
{
    internal class RemoveChecklistCommandHandler : RemoveCommandHandler<Checklist>
    {
        public RemoveChecklistCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }

        public override async Task<Unit> Handle(RemoveCommand<Checklist> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            UnitOfWork.Repository.RemoveChecklist(request.Id);
            await UnitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
