﻿
using ToDoList.Core.Entities;
using ToDoList.Core.Handlers.Generics;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Images
{
    internal class UpdateImageCommandHandler : UpdateCommandHandler<Image>
    {
        public UpdateImageCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
