using System;
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
    class GetProjectByNameAndUserIdQueryHandler : HandlerBase, IRequestHandler<GetProjectByNameAndUserIdQuery, ProjectResponse>
    {
        public GetProjectByNameAndUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public Task<ProjectResponse> Handle(GetProjectByNameAndUserIdQuery request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            return Task.FromResult(UnitOfWork.Repository.GetAll<Project>()
                                                        .Where(x => x.Name == request.Name && x.UserId == request.UserId)
                                                        .ProjectTo<ProjectResponse>(Mapper.ConfigurationProvider)
                                                        .SingleOrDefault());
        }
    }
}
