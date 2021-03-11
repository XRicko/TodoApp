
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
    internal class GetTodoItemByIdQueryHandler : GetByIdQueryHandler<TodoItem, TodoItemResponse>
    {
        private readonly ICreateTodoItemResponseWithAddressService createAddressService;

        public GetTodoItemByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICreateTodoItemResponseWithAddressService addressService) : base(unitOfWork, mapper)
        {
            createAddressService = addressService;
        }

        public override async Task<TodoItemResponse> Handle(GetByIdQuery<TodoItem, TodoItemResponse> request, CancellationToken cancellationToken)
        {
            var response = await base.Handle(request, cancellationToken);
            var responseWithAddress = await createAddressService.GetTodoItemResponseWithAddress(response);

            return responseWithAddress;
        }
    }
}
