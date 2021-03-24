using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Queries.Checklists;
using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Checklists
{
    public class GetChecklistsByUserIdQueryHandler : HandlerBase, IRequestHandler<GetChecklistsByUserIdQuery, IEnumerable<ChecklistResponse>>
    {
        public GetChecklistsByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<IEnumerable<ChecklistResponse>> Handle(GetChecklistsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var checklists = await UnitOfWork.Repository.GetAllAsync<Checklist>();
            var checklistsByUser = checklists.Where(x => x.UserId == request.UserId);

            return Mapper.Map<IEnumerable<ChecklistResponse>>(checklistsByUser);
        }
    }
}
