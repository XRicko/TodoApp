﻿
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Images
{
    internal class RemoveImageCommandHandler : RemoveCommandHandler<Image>
    {
        public RemoveImageCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
