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
    internal class RemoveChecklistByIdCommandHandler : RemoveByIdCommandHandler<Checklist>
    {
        public RemoveChecklistByIdCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }

        public override async Task<Unit> Handle(RemoveByIdCommand<Checklist> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            UnitOfWork.Repository.RemoveChecklist(request.Id);
            await UnitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
