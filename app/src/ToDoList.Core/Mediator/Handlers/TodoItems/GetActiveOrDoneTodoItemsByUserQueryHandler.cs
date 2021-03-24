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
    public class GetActiveOrDoneTodoItemsByUserQueryHandler : HandlerBase, IRequestHandler<GetActiveOrDoneTodoItemsByUserQuery, IEnumerable<TodoItemResponse>>
    {
        private readonly ICreateWithAddressService createWithAddressService;

        public GetActiveOrDoneTodoItemsByUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICreateWithAddressService addressService) : base(unitOfWork, mapper)
        {
            createWithAddressService = addressService;
        }

        public async Task<IEnumerable<TodoItemResponse>> Handle(GetActiveOrDoneTodoItemsByUserQuery request, CancellationToken cancellationToken)
        {
            var todoItems = await UnitOfWork.Repository.GetAllAsync<TodoItem>();
            var activeOrDoneTodoItemsByUser = todoItems.Where(i => i.Checklist.UserId == request.UserId
                                                             && i.Status.IsDone == request.IsDone);

            var responses = Mapper.Map<IEnumerable<TodoItemResponse>>(activeOrDoneTodoItemsByUser);
            var responsesWithAddress = await createWithAddressService.GetItemsWithAddressAsync(responses);

            return responsesWithAddress;
        }
    }
}
