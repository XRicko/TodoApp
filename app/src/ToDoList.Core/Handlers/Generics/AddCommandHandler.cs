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
            T item = await unitOfWork.Repository.GetAsync(request.Entity);

            if (item is null)
            {
                await unitOfWork.Repository.AddAsync(request.Entity);
                await unitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
