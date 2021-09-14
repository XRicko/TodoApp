using System;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using ToDoList.Core.Entities;
using ToDoList.Core.Extensions;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.Core.Services;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.TodoItems
{
    internal class GetTodoItemByIdQueryHandler : GetByIdQueryHandler<TodoItem, TodoItemResponse>
    {
        private readonly IAddressService createAddressService;
        private readonly IFileSystem fileSystem;

        public GetTodoItemByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
                                           IAddressService addressService, IFileSystem system) : base(unitOfWork, mapper)
        {
            createAddressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
            fileSystem = system ?? throw new ArgumentNullException(nameof(system));
        }

        public override async Task<TodoItemResponse> Handle(GetByIdQuery<TodoItem, TodoItemResponse> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var response = UnitOfWork.Repository.GetAll<TodoItem>()
                                                .Where(x => x.Id == request.Id)
                                                .AsTodoItemResponses(Mapper, fileSystem)
                                                .SingleOrDefault();

            return await createAddressService.GetItemWithAddressAsync(response);
        }
    }
}
