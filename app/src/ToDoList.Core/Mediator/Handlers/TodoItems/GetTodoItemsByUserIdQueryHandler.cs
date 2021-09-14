using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Entities;
using ToDoList.Core.Extensions;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Queries.TodoItems;
using ToDoList.Core.Mediator.Response;
using ToDoList.Core.Services;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.TodoItems
{
    internal class GetTodoItemsByUserIdQueryHandler : HandlerBase, IRequestHandler<GetTodoItemsByUserIdQuery, IEnumerable<TodoItemResponse>>
    {
        private readonly IAddressService createWithAddressService;
        private readonly IFileSystem fileSystem;

        public GetTodoItemsByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
                                                IAddressService addressService, IFileSystem system) : base(unitOfWork, mapper)
        {
            createWithAddressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
            fileSystem = system ?? throw new ArgumentNullException(nameof(system));
        }

        public async Task<IEnumerable<TodoItemResponse>> Handle(GetTodoItemsByUserIdQuery request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var responsesByUser = UnitOfWork.Repository.GetAll<TodoItem>()
                                                       .Where(x => x.Checklist.UserId == request.UserId)
                                                       .AsTodoItemResponses(Mapper, fileSystem)
                                                       .ToList();

            return await createWithAddressService.GetItemsWithAddressAsync(responsesByUser);
        }
    }
}
