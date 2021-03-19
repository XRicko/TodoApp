using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.TodoItems
{
    internal class GetTodoItemsByUserQueryHandler : HandlerBase, IRequestHandler<GetByUserIdQuery<TodoItem, TodoItemResponse>, IEnumerable<TodoItemResponse>>
    {
        public GetTodoItemsByUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<IEnumerable<TodoItemResponse>> Handle(GetByUserIdQuery<TodoItem, TodoItemResponse> request, CancellationToken cancellationToken)
        {
            var todoItems = await UnitOfWork.Repository.GetAllAsync<TodoItem>();
            var todoItemsByUser = todoItems.Where(i => i.Checklist.UserId == request.UserId);

            return Mapper.Map<IEnumerable<TodoItemResponse>>(todoItemsByUser);
        }
    }
}
