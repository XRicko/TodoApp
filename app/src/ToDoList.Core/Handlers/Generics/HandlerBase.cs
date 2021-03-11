using System;

using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Generics
{
    internal abstract class HandlerBase
    {
        protected IUnitOfWork UnitOfWork { get; }

        public HandlerBase(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
    }
}
