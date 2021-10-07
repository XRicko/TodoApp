using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Commands.Generics;
using ToDoList.Core.Mediator.Handlers.Generics;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.SharedKernel;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Mediator.Handlers.Projects
{
    internal class AddProjectCommandHandler : AddCommandHandler<ProjectCreateRequest, Project>
    {
        public AddProjectCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public override async Task<Unit> Handle(AddCommand<ProjectCreateRequest> request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            bool recordExists = UnitOfWork.Repository.GetAll<Project>()
                                                     .Any(x => x.Name == request.Request.Name
                                                               && x.UserId == request.Request.UserId);

            if (!recordExists)
            {
                var entity = Mapper.Map<Project>(request.Request);

                UnitOfWork.Repository.Add(entity);
                await UnitOfWork.SaveAsync();

                var defaultChecklist = new Checklist { Name = Constants.Untitled, ProjectId = entity.Id };

                UnitOfWork.Repository.Add(defaultChecklist);
                await UnitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
