using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.SharedKernel;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Generics
{
    internal class RemoveByIdCommandHandler<TEntity> : HandlerBase, IRequestHandler<RemoveByIdCommand<TEntity>>
        where TEntity : BaseEntity, new()
    {
        public RemoveByIdCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        public virtual async Task<Unit> Handle(RemoveByIdCommand<TEntity> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var entity = UnitOfWork.Repository.GetAll<TEntity>()
                                              .Select(x => new TEntity { Id = x.Id })
                                              .SingleOrDefault(x => x.Id == request.Id);

            if (entity is not null)
            {
                UnitOfWork.Repository.Remove(entity);
                await UnitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
