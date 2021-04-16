using System;
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
using ToDoList.Core.Services;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.TodoItems
{
    public class GetTodoItemsByUserIdQueryHandler : HandlerBase, IRequestHandler<GetTodoItemsByUserIdQuery, IEnumerable<TodoItemResponse>>
    {
        private readonly ICreateWithAddressService createWithAddressService;

        public GetTodoItemsByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICreateWithAddressService addressService) : base(unitOfWork, mapper)
        {
            createWithAddressService = addressService;
        }

        public async Task<IEnumerable<TodoItemResponse>> Handle(GetTodoItemsByUserIdQuery request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var todoItems = await UnitOfWork.Repository.GetAllAsync<TodoItem>();
            var todoItemsByUser = todoItems.Where(i => i.Checklist.UserId == request.UserId);

            var responses = Mapper.Map<IEnumerable<TodoItemResponse>>(todoItemsByUser);
            var responsesWithAddress = await createWithAddressService.GetItemsWithAddressAsync(responses);

            return responsesWithAddress;
        }
    }
}
