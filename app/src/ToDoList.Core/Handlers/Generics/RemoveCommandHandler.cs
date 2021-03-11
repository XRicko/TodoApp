﻿using System.Threading;
using System.Threading.Tasks;

using MediatR;

using ToDoList.Core.Commands;
using ToDoList.SharedKernel;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Generics
{
    internal class RemoveCommandHandler<T> : HandlerBase, IRequestHandler<RemoveCommand<T>> where T : BaseEntity
    {
        public RemoveCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<Unit> Handle(RemoveCommand<T> request, CancellationToken cancellationToken)
        {
            T entity = await UnitOfWork.Repository.GetAsync<T>(request.Id);

            if (entity is not null)
            {
                UnitOfWork.Repository.Remove(entity);
                await UnitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
