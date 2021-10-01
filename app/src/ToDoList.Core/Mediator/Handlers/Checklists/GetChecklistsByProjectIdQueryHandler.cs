using System;
using System.Collections.Generic;
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
    internal class GetChecklistsByProjectIdQueryHandler : HandlerBase, IRequestHandler<GetChecklistsByProjectIdQuery, IEnumerable<ChecklistResponse>>
    {
        public GetChecklistsByProjectIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<IEnumerable<ChecklistResponse>> Handle(GetChecklistsByProjectIdQuery request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            return await Task.FromResult(UnitOfWork.Repository.GetAll<Checklist>()
                                                              .Where(x => x.ProjectId == request.ProjectId)
                                                              .ProjectTo<ChecklistResponse>(Mapper.ConfigurationProvider)
                                                              .ToList());
        }
    }
}
