using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Queries;
using ToDoList.Core.Mediator.Response;
using ToDoList.Core.Services;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.TodoItems
{
    internal class GetTodoItemsQueryHandler : GetAllQueryHandler<TodoItem, TodoItemResponse>
    {
        private readonly ICreateWithAddressService createAddressService;

        public GetTodoItemsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICreateWithAddressService addressService) : base(unitOfWork, mapper)
        {
            createAddressService = addressService;
        }

        public override async Task<IEnumerable<TodoItemResponse>> Handle(GetAllQuery<TodoItem, TodoItemResponse> request, CancellationToken cancellationToken)
        {
            var responses = await base.Handle(request, cancellationToken);
            var responsesWithAddress = await createAddressService.GetItemsWithAddressAsync(responses);

            return responsesWithAddress;
        }
    }
}
