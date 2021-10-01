using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Queries.Checklists;
using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Checklists
{
    internal class GetChecklistByNameAndProjectIdQueryHandler : HandlerBase, IRequestHandler<GetChecklistByNameAndProjectIdQuery, ChecklistResponse>
    {
        public GetChecklistByNameAndProjectIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public Task<ChecklistResponse> Handle(GetChecklistByNameAndProjectIdQuery request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            return Task.FromResult(UnitOfWork.Repository.GetAll<Checklist>()
                                                        .Where(x => x.Name == request.Name && x.ProjectId == request.ProjectId)
                                                        .ProjectTo<ChecklistResponse>(Mapper.ConfigurationProvider)
                                                        .SingleOrDefault());
        }
    }
}
