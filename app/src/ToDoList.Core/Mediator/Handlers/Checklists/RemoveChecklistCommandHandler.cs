using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Entities;
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

            var checklist = UnitOfWork.Repository.GetAll<Checklist>()
                                                 .Select(x => new Checklist
                                                 {
                                                     Id = x.Id,
                                                     Name = x.Name,
                                                     TodoItems = x.TodoItems.Select(x => new TodoItem
                                                     {
                                                         Id = x.Id,
                                                         ChecklistId = x.ChecklistId
                                                     }).ToList()
                                                 })
                                                 .SingleOrDefault(x => x.Id == request.Id);

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
