using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ToDoList.Core.Queries;
using ToDoList.SharedKernel;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers
{
    public class GetByIdHandler<T> : HandlerBase, IRequestHandler<GetByIdQuery<T>, T> where T : BaseEntity
    {
        public GetByIdHandler(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<T> Handle(GetByIdQuery<T> request, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.Repository.GetAsync<T>(request.Id);
            return result;
        }
    }
}
