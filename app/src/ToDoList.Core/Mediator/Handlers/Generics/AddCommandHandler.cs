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
    internal class AddCommandHandler<TRequest, TEntity> : HandlerBase, IRequestHandler<AddCommand<TRequest>>
        where TRequest : BaseRequest
        where TEntity : BaseEntity
    {
        public AddCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        public virtual async Task<Unit> Handle(AddCommand<TRequest> request, CancellationToken cancellationToken)
        {
            var entity = Mapper.Map<TEntity>(request.Request);
            var item = await UnitOfWork.Repository.GetAsync<TEntity>(entity.Name);

            if (item is null)
            {
                await UnitOfWork.Repository.AddAsync(entity);
                await UnitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
