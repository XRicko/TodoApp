using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.Core.Services;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.TodoItems
{
    internal class GetTodoItemByIdQueryHandler : GetByIdQueryHandler<TodoItem, TodoItemResponse>
    {
        private readonly ICreateWithAddressService createAddressService;

        public GetTodoItemByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICreateWithAddressService addressService) : base(unitOfWork, mapper)
        {
            createAddressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
        }

        public override async Task<TodoItemResponse> Handle(GetByIdQuery<TodoItem, TodoItemResponse> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var response = await base.Handle(request, cancellationToken);
            var responseWithAddress = await createAddressService.GetItemWithAddressAsync(response);

            return responseWithAddress;
        }
    }
}
