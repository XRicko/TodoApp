using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using ToDoList.Core.Queries;
using ToDoList.SharedKernel;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Generics
{
    internal abstract class GetAllQueryHandler<T> : HandlerBase, IRequestHandler<GetAllQuery<T>, IEnumerable<T>> where T : BaseEntity
    {
        public GetAllQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<IEnumerable<T>> Handle(GetAllQuery<T> request, CancellationToken cancellationToken)
        {
            var result = await UnitOfWork.Repository.GetAllAsync<T>();
            return result;
        }
    }
}
