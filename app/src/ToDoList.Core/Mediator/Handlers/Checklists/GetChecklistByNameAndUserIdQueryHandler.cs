using System;
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
    public class GetChecklistByNameAndUserIdQueryHandler : HandlerBase, IRequestHandler<GetChecklistByNameAndUserIdQuery, ChecklistResponse>
    {
        public GetChecklistByNameAndUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public Task<ChecklistResponse> Handle(GetChecklistByNameAndUserIdQuery request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var entity = UnitOfWork.Repository.GetAll<Checklist>()
                                              .SingleOrDefault(x => x.Name == request.Name && x.UserId == request.UserId);

            return Task.FromResult(Mapper.Map<ChecklistResponse>(entity));
        }
    }
}
