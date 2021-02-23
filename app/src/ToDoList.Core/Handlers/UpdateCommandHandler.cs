using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ToDoList.Core.Commands;
using ToDoList.SharedKernel;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers
{
    public class UpdateCommandHandler<T> : HandlerBase, IRequestHandler<UpdateCommand<T>> where T : BaseEntity
    {
        public UpdateCommandHandler(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<Unit> Handle(UpdateCommand<T> request, CancellationToken cancellationToken)
        {
            unitOfWork.Repository.Update(request.Entity);
            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
