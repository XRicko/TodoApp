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

namespace ToDoList.Core.Mediator.Handlers.TodoItems
{
    public class AddTodoItemCommandHandler : AddCommandHandler<TodoItemCreateRequest, TodoItem>
    {
        public AddTodoItemCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }

        public override async Task<Unit> Handle(AddCommand<TodoItemCreateRequest> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            bool recordExists = UnitOfWork.Repository.GetAll<TodoItem>()
                                                     .Any(e => e.Name == request.Request.Name
                                                               && e.ChecklistId == request.Request.ChecklistId);

            if (!recordExists)
            {
                var entity = Mapper.Map<TodoItem>(request.Request);

                UnitOfWork.Repository.Add(entity);
                await UnitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
