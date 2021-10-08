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
    internal class RemoveByNameCommandHandler<TEntity> : HandlerBase, IRequestHandler<RemoveByNameCommand<TEntity>>
        where TEntity : BaseEntity, new()
    {
        public RemoveByNameCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<Unit> Handle(RemoveByNameCommand<TEntity> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var entity = UnitOfWork.Repository.GetAll<TEntity>()
                                              .Select(x => new TEntity { Id = x.Id, Name = x.Name })
                                              .SingleOrDefault(x => x.Name == request.Name);

            if (entity is not null)
            {
                UnitOfWork.Repository.Remove(entity);
                await UnitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
