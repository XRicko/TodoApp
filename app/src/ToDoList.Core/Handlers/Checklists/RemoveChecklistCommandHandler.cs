﻿
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Checklists
{
    class RemoveChecklistCommandHandler : RemoveCommandHandler<Checklist>
    {
        public RemoveChecklistCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}