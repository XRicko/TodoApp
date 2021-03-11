using System.Threading;
using System.Threading.Tasks;

using MediatR;

using ToDoList.Core.Commands;
using ToDoList.SharedKernel;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Generics
{
    internal class AddCommandHandler<T> : HandlerBase, IRequestHandler<AddCommand<T>> where T : BaseEntity
    {
        public AddCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<Unit> Handle(AddCommand<T> request, CancellationToken cancellationToken)
        {
            T item = await UnitOfWork.Repository.GetAsync(request.Entity);

            if (item is null)
            {
                await UnitOfWork.Repository.AddAsync(request.Entity);
                await UnitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
