using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Mediator.Commands;
using ToDoList.Core.Mediator.Requests;
using ToDoList.SharedKernel;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Generics
{
    internal class UpdateCommandHandler<TRequest, TEntity> : HandlerBase, IRequestHandler<UpdateCommand<TRequest>>
        where TRequest : BaseRequest
        where TEntity : BaseEntity
    {
        public UpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        public async Task<Unit> Handle(UpdateCommand<TRequest> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var entity = Mapper.Map<TEntity>(request.Request);

            UnitOfWork.Repository.Update(entity);
            await UnitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
