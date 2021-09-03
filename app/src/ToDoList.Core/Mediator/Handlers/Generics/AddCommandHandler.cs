using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Requests;
using ToDoList.SharedKernel;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Generics
{
    internal class AddCommandHandler<TRequest, TEntity> : HandlerBase, IRequestHandler<AddCommand<TRequest>>
        where TRequest : BaseRequest
        where TEntity : BaseEntity
    {
        public AddCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        public virtual async Task<Unit> Handle(AddCommand<TRequest> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            bool itemExists = UnitOfWork.Repository.GetAll<TEntity>()
                                                   .Any(x => x.Name == request.Request.Name);

            if (!itemExists)
            {
                var entity = Mapper.Map<TEntity>(request.Request);

                UnitOfWork.Repository.Add(entity);
                await UnitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
