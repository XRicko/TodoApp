using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.Core.Services;
using ToDoList.SharedKernel;
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

            var response = UnitOfWork.Repository.GetAll<TodoItem>()
                                                .Where(x => x.Id == request.Id)
                                                .Select(x => new TodoItemResponse(x.Id, x.Name, x.StartDate,
                                                                                  x.ChecklistId, x.Checklist.Name,
                                                                                  x.StatusId, x.Status.Name, x.DueDate,
                                                                                  Mapper.Map<GeoCoordinate>(x.GeoPoint),
                                                                                  x.CategoryId, x.Category.Name,
                                                                                  x.ImageId, x.Image.Name, x.Image.Path))
                                                .SingleOrDefault();

            return await createAddressService.GetItemWithAddressAsync(response);
        }
    }
}
