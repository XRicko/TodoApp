﻿using System;

using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Generics
{
    internal abstract class HandlerBase
    {
        protected readonly IUnitOfWork unitOfWork;

        public HandlerBase(IUnitOfWork unit)
        {
            unitOfWork = unit ?? throw new ArgumentNullException(nameof(unit));
        }
    }
}