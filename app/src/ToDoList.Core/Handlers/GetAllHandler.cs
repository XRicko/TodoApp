using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ToDoList.Core.Queries;
using ToDoList.SharedKernel;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers
{
    public class GetAllHandler<T> : HandlerBase, IRequestHandler<GetAllQuery<T>, IEnumerable<T>> where T : BaseEntity
    {
        public GetAllHandler(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<IEnumerable<T>> Handle(GetAllQuery<T> request, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.Repository.GetAllAsync<T>();
            return result;
        }
    }
}
