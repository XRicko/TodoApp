using MediatR;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Mediator.Queries.Projects
{
    public record GetProjectByNameAndUserIdQuery(string Name, int UserId) : IRequest<ProjectResponse>;
}
