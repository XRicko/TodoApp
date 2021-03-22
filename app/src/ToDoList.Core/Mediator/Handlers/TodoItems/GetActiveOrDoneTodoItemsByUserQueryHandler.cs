using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Queries.TodoItems;
using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.TodoItems
{
    internal class GetActiveOrDoneTodoItemsByUserQueryHandler : HandlerBase, IRequestHandler<GetActiveOrDoneTodoItemsByUserQuery, IEnumerable<TodoItemResponse>>
    {
        public GetActiveOrDoneTodoItemsByUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<IEnumerable<TodoItemResponse>> Handle(GetActiveOrDoneTodoItemsByUserQuery request, CancellationToken cancellationToken)
        {
            var todoItems = await UnitOfWork.Repository.GetAllAsync<TodoItem>();
            var activeTodoItemsByUser = todoItems.Where(i => i.Checklist.UserId == request.UserId
                                                             && i.Status.IsDone == request.IsDone);

            return Mapper.Map<IEnumerable<TodoItemResponse>>(activeTodoItemsByUser);
        }
    }
}
