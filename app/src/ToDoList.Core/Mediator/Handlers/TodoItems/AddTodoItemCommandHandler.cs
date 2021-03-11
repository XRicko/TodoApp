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
        public AddTodoItemCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }

        public override async Task<Unit> Handle(AddCommand<TodoItemCreateRequest> request, CancellationToken cancellationToken)
        {
            var entity = Mapper.Map<TodoItem>(request.Request);
            var todoItem = await UnitOfWork.Repository.GetAsync(entity);

            if (todoItem is null)
            {
                todoItem.GeoPoint.SRID = 4326;

                await UnitOfWork.Repository.AddAsync(entity);
                await UnitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
