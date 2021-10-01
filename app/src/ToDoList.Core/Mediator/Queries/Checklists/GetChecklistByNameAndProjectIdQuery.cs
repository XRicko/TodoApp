using MediatR;

using ToDoList.Core.Mediator.Response;

namespace ToDoList.Core.Mediator.Queries.Checklists
{
    public record GetChecklistByNameAndProjectIdQuery(string Name, int ProjectId) : IRequest<ChecklistResponse>;
}
