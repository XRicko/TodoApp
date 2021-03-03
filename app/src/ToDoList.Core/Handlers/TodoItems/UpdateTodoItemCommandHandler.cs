﻿using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.TodoItems
{
    internal class UpdateTodoItemCommandHandler : UpdateCommandHandler<TodoItem>
    {
        public UpdateTodoItemCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
