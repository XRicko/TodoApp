﻿using System.Diagnostics.CodeAnalysis;

using AutoMapper;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Images
{
    internal class AddImageCommandHandler : AddCommandHandler<ImageCreateRequest, Image>
    {
        [ExcludeFromCodeCoverage]
        public AddImageCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }
    }
}
