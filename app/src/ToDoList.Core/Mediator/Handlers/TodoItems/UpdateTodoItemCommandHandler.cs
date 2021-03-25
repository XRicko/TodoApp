using System.Diagnostics.CodeAnalysis;

using AutoMapper;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.TodoItems
{
    [ExcludeFromCodeCoverage]
    internal class UpdateTodoItemCommandHandler : UpdateCommandHandler<TodoItemUpdateRequest, TodoItem>
    {
        public UpdateTodoItemCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }
    }
}
