using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Checklists
{
    public class RemoveChecklistCommandHandler : RemoveCommandHandler<Checklist>
    {
        public RemoveChecklistCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }

        public override async Task<Unit> Handle(RemoveCommand<Checklist> request, CancellationToken cancellationToken)
        {
            var checklist = await UnitOfWork.Repository.GetAsync<Checklist>(request.Id);

            if (checklist is not null && checklist.Name != "Untitled")
            {
                foreach (var item in checklist.TodoItems)
                {
                    UnitOfWork.Repository.Remove(item);
                }
                UnitOfWork.Repository.Remove(checklist);

                await UnitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
