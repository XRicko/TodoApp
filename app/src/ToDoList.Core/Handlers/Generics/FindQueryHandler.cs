using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using ToDoList.Core.Queries;
using ToDoList.SharedKernel;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Handlers.Generics
{
    internal class FindQueryHandler<T> : HandlerBase, IRequestHandler<FindQuery<T>, IEnumerable<T>> where T : BaseEntity
    {
        public FindQueryHandler(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public Task<IEnumerable<T>> Handle(FindQuery<T> request, CancellationToken cancellationToken)
        {
            var result = unitOfWork.Repository.FindAsync(request.Predicate);
            return result;
        }
    }
}
