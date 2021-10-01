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
using ToDoList.Core.Mediator.Queries.Projects;
using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Projects
{
    internal class GetProjectsByUserIdQueryHandler : HandlerBase, IRequestHandler<GetProjectsByUserIdQuery, IEnumerable<ProjectResponse>>
    {
        public GetProjectsByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<IEnumerable<ProjectResponse>> Handle(GetProjectsByUserIdQuery request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            return await Task.FromResult(UnitOfWork.Repository.GetAll<Project>()
                                                              .Where(x => x.UserId == request.UserId)
                                                              .ProjectTo<ProjectResponse>(Mapper.ConfigurationProvider)
                                                              .ToList());
        }
    }
}
