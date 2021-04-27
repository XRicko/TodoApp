using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Mediator.Commands;
using ToDoList.SharedKernel;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Generics
{
    public class RemoveCommandHandler<TEntity> : HandlerBase, IRequestHandler<RemoveCommand<TEntity>>
        where TEntity : BaseEntity
    {
        public RemoveCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        public virtual async Task<Unit> Handle(RemoveCommand<TEntity> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var entity = await UnitOfWork.Repository.FindByPrimaryKeysAsync<TEntity>(request.Id);

            if (entity is not null)
            {
                UnitOfWork.Repository.Remove(entity);
                await UnitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
