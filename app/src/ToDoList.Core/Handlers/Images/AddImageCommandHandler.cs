﻿
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Images
{
    internal class AddImageCommandHandler : AddCommandHandler<Image>
    {
        public AddImageCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
