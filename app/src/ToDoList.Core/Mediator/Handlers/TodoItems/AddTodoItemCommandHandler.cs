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

namespace ToDoList.Core.Mediator.Handlers.TodoItems
{
    internal class AddTodoItemCommandHandler : AddCommandHandler<TodoItemCreateRequest, TodoItem>
    {
        private const int SRID = 4326;

        public AddTodoItemCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }

        public override async Task<Unit> Handle(AddCommand<TodoItemCreateRequest> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var todoItems = await UnitOfWork.Repository.GetAllAsync<TodoItem>();
            var todoItem = todoItems.SingleOrDefault(e => e.Name == request.Request.Name
                                                          && e.ChecklistId == request.Request.ChecklistId);

            if (todoItem is null)
            {
                var entity = Mapper.Map<TodoItem>(request.Request);

                if (entity.GeoPoint is not null)
                    entity.GeoPoint.SRID = SRID;

                await UnitOfWork.Repository.AddAsync(entity);
                await UnitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
