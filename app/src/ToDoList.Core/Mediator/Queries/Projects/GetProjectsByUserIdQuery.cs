using System.Collections.Generic;

using MediatR;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Mediator.Queries.Projects
{
    public record GetProjectsByUserIdQuery(int UserId) : IRequest<IEnumerable<ProjectResponse>>;
}
